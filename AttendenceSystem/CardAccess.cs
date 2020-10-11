using PCSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AttendenceSystem
{
    class CardAccess
    {
        SelectFrmDb sdb = new SelectFrmDb();
        static bool CheckErr(SCardError err)
        {
            if (err != SCardError.Success)
            {
                throw new PCSCException(err, SCardHelper.StringifyError(err));
            }
            else
            {
                return true;
            }
        }
        static bool CheckRespnse(byte[] pbRecvBuffer)
        {
            string response = "";
            for (int i = 0; i < pbRecvBuffer.Length; i++)
            {
                if ((i + 2) >= pbRecvBuffer.Length)
                {
                    response = response + " " + string.Format("{0:X2}", pbRecvBuffer[i]);
                }
            }
            if ((response).Trim() != "90 00")
            {
                return false;
                throw new Exception("Operation Failed during authentication");
            }
            else
            {
                return true;
            }
        }
        public SCardContext EstablishContext()
        {
            // Establish SCard context
            SCardContext hContext = new SCardContext();
            hContext.Establish(SCardScope.System);
            return hContext;
        }
        public string[] RetrieveReaders(SCardContext hContext)
        {
            // Retrieve the list of Smartcard readers
            string[] szReaders = hContext.GetReaders();

            if (szReaders.Length <= 0)
                throw new PCSCException(SCardError.NoReadersAvailable,
                    "Could not find any Smartcard reader.");

            //            Console.WriteLine("reader name: " + szReaders[0]);
            return szReaders;
        }

        public bool LoadAuth(SCardError err, SCardReader reader, IntPtr pioSendPci)
        {
            byte[] pbRecvBuffer = new byte[256];

            // load auth key reader (page 12)
            //FF=class, 82=ins, 00=key structure(p1), 00=key no p2 (location), 06=byte, (FF, FF, FF, FF, FF, FF)=key value loaded in reader
            byte[] cmd2 = new byte[] { (byte)0xff, (byte)0x82, 00, 01, 06, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF };

            err = reader.Transmit(pioSendPci, cmd2, ref pbRecvBuffer);
            CheckErr(err);

            Console.Write("load authkey to reader: ");
            string retstr = "";
            for (int i = 0; i < pbRecvBuffer.Length; i++)
            {
                retstr = retstr + " " + string.Format("{0:X2}", pbRecvBuffer[i]);
                Console.Write("{0:X2} ", pbRecvBuffer[i]);
            }
            Console.WriteLine();

            if ((retstr).Trim() == "90 00")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StoreId(SCardError err, SCardReader reader, IntPtr pioSendPci, string block, string str_id)
        {
            int id = Convert.ToInt32(str_id);
            string to_hex = id.ToString("X8");
            string substr1 = to_hex.Substring(0, 2);
            string substr2 = to_hex.Substring(2, 2);
            string substr3 = to_hex.Substring(4, 2);
            string substr4 = to_hex.Substring(6, 2);

            byte[] pbRecvBuffer = new byte[256];
            //store value hex id into block
            byte[] cmd4 = new byte[] { (byte)0xFF, (byte)0xD7, (byte)0x00, (byte)int.Parse(block), (byte)0x05, (byte)0x00, (byte)int.Parse(substr1), (byte)int.Parse(substr2), (byte)int.Parse(substr3), (byte)int.Parse(substr4) };

            err = reader.Transmit(pioSendPci, cmd4, ref pbRecvBuffer);
            CheckErr(err);
            if (CheckRespnse(pbRecvBuffer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ReadUid(SCardError err, SCardReader reader, IntPtr pioSendPci, string block)
        {
            byte[] pbRecvBuffer = new byte[256];
            byte[] cmd4 = new byte[] { (byte)0xFF, (byte)0xCA, (byte)0x00, (byte)0x00, (byte)0x00 };

            err = reader.Transmit(pioSendPci, cmd4, ref pbRecvBuffer);
            CheckErr(err);
            string carduid = BitConverter.ToString(pbRecvBuffer.Take(4).ToArray()).Replace("-", string.Empty).ToUpper();
            pbRecvBuffer = new byte[256];

            //to read stored value from block
            byte[] cmd5 = new byte[] { (byte)0xFF, (byte)0xB1, (byte)0x00, (byte)int.Parse(block), (byte)0x04 };
            err = reader.Transmit(pioSendPci, cmd5, ref pbRecvBuffer);
            CheckErr(err);

            string hex_uid = "";
            if (pbRecvBuffer.Length > 2)
            {
                for (int i = 0; i < pbRecvBuffer.Length - 2; i++)
                {
                    hex_uid = hex_uid + string.Format("{0:X2}", pbRecvBuffer[i]);
                }
                int in_uid = Convert.ToInt32(hex_uid);
                sdb.RegisterAttendence(in_uid, carduid);
            }
            else
            {
                MessageBox.Show("User Data Not Found");
            }
        }

        public bool RegisterCard(string str_id)
        {
            SCardError err;
            IntPtr pioSendPci;
            bool status = false;
            try
            {
                bool store_status = false;
                SCardContext hContext = EstablishContext();
                string[] szReaders = RetrieveReaders(hContext);
                // Create a reader object using the existing context
                SCardReader reader = new SCardReader(hContext);

                // Connect to the card
                err = reader.Connect(szReaders[0], SCardShareMode.Shared, SCardProtocol.T0 | SCardProtocol.T1);
                CheckErr(err);

                switch (reader.ActiveProtocol)
                {
                    case SCardProtocol.T0:
                        pioSendPci = SCardPCI.T0;
                        break;
                    case SCardProtocol.T1:
                        pioSendPci = SCardPCI.T1;
                        break;
                    default:
                        throw new PCSCException(SCardError.ProtocolMismatch,
                            "Protocol not supported: "
                            + reader.ActiveProtocol.ToString());
                }

                byte[] pbRecvBuffer = new byte[256];

                bool authvalid = LoadAuth(err, reader, pioSendPci);

                if (authvalid == true)
                {
                    string block = "10";
                    //to authenticate, uses the keys stored in reader (previous step 6 ff) to do authentication. two types of authentication type A = 60 and type B = 61.
                    //FF=class, 86=ins, 00=p1, 00=p2, 05=LC, (00=(version 00 or 01), 00, 08=block no to be authenticated, 61=(key type 60 for A or 61 for B), 00=(key location or number 00~01)) = authentication data bytes.
                    // this APDU is PC/SC V2.07
                    byte[] cmd3 = new byte[] { (byte)0xFF, (byte)0x86, (byte)0x00, (byte)0x00, (byte)0x05, (byte)0x00, (byte)0x00, (byte)int.Parse(block), (byte)0x61, (byte)0x00 };

                    err = reader.Transmit(pioSendPci, cmd3, ref pbRecvBuffer);
                    CheckErr(err);

                    CheckRespnse(pbRecvBuffer);
                    store_status = StoreId(err, reader, pioSendPci, block, str_id);
                }
                if (store_status == true)
                {
                    //database update here
                    pbRecvBuffer = new byte[256];
                    byte[] cmd4 = new byte[] { (byte)0xFF, (byte)0xCA, (byte)0x00, (byte)0x00, (byte)0x00 };

                    err = reader.Transmit(pioSendPci, cmd4, ref pbRecvBuffer);
                    CheckErr(err);
                    string carduid = BitConverter.ToString(pbRecvBuffer.Take(4).ToArray()).Replace("-", string.Empty).ToUpper();
                    if (sdb.AfterCardCreate(Convert.ToInt32(str_id), carduid))
                        status = true;
                    else
                    {
                        status = false;
                        throw new Exception("Error During Saving");
                    }
                }
                hContext.Release();
                return status;
            }
            catch (PCSCException ex)
            {
                MessageBox.Show("Error: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
                return false;
            }
        }


        public string GetCardId(SCardError err, SCardReader reader, IntPtr pioSendPci)
        {
            byte[] pbRecvBuffer = new byte[256];
            //store value hex id into block
            byte[] cmd4 = new byte[] { (byte)0xFF, (byte)0xCA, (byte)0x00, (byte)0x00, (byte)0x00 };

            err = reader.Transmit(pioSendPci, cmd4, ref pbRecvBuffer);
            //            CheckErr(err);
            return BitConverter.ToString(pbRecvBuffer.Take(4).ToArray()).Replace("-", string.Empty);
        }

        public void KeepWaiting(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                SCardError err;
                IntPtr pioSendPci;
                try
                {
                    SCardContext hContext = EstablishContext();
                    string[] szReaders = RetrieveReaders(hContext);
                    // Create a reader object using the existing context
                    SCardReader reader = new SCardReader(hContext);

                    // Connect to the card
                    err = reader.Connect(szReaders[0], SCardShareMode.Shared, SCardProtocol.T0 | SCardProtocol.T1);
                    if (err != SCardError.Success)
                    {
                        throw new PCSCException(err, SCardHelper.StringifyError(err));
                    }
                    else
                    {
                        SmartcardState cstate = CheckCardState();
                        if (cstate == SmartcardState.CardChanged)
                        {
                            switch (reader.ActiveProtocol)
                            {
                                case SCardProtocol.T0:
                                    pioSendPci = SCardPCI.T0;
                                    break;
                                case SCardProtocol.T1:
                                    pioSendPci = SCardPCI.T1;
                                    break;
                                default:
                                    throw new PCSCException(SCardError.ProtocolMismatch,
                                        "Protocol not supported: "
                                        + reader.ActiveProtocol.ToString());
                            }

                            bool authvalid = LoadAuth(err, reader, pioSendPci);
                            if (authvalid == true)
                            {
                                string block = "10";
                                byte[] pbRecvBuffer = new byte[256];
                                //to authenticate, uses the keys stored in reader (previous step 6 ff) to do authentication. two types of authentication type A = 60 and type B = 61.
                                //FF=class, 86=ins, 00=p1, 00=p2, 05=LC, (00=(version 00 or 01), 00, 08=block no to be authenticated, 61=(key type 60 for A or 61 for B), 00=(key location or number 00~01)) = authentication data bytes.
                                // this APDU is PC/SC V2.07
                                byte[] cmd3 = new byte[] { (byte)0xFF, (byte)0x86, (byte)0x00, (byte)0x00, (byte)0x05, (byte)0x00, (byte)0x00, (byte)int.Parse(block), (byte)0x61, (byte)0x00 };

                                err = reader.Transmit(pioSendPci, cmd3, ref pbRecvBuffer);
                                CheckErr(err);

                                if (CheckRespnse(pbRecvBuffer))
                                {
                                    ReadUid(err, reader, pioSendPci, block);
                                }
                            }
                        }
                    }
                    hContext.Release();
                }
                catch (PCSCException ex)
                {
                    Console.WriteLine("Error: "
                        + ex.Message
                        + " (" + ex.SCardError.ToString() + ")");
                }
            }
        }

        internal enum SmartcardState
        {
            CardChanged = 0,
            CardNotChanged = 1,
        }

        public int cardcnt;
        public SmartcardState CheckCardState()
        {
            using (var ctx = new SCardContext())
            {
                ctx.Establish(SCardScope.System); var readerNames = ctx.GetReaders(); // Get the current status for all readers. 
                var states = ctx.GetReaderStatus(readerNames);
                SmartcardState cstate = SmartcardState.CardNotChanged;
                foreach (var state in states)
                {

                    if (cardcnt != state.CardChangeEventCnt)
                    {
                        cardcnt = state.CardChangeEventCnt;
                        cstate = SmartcardState.CardChanged;
                    }
                    else
                    {
                        cardcnt = state.CardChangeEventCnt;
                        cstate = SmartcardState.CardNotChanged;
                    }
                }
                return cstate;
            }
        }
    }
}