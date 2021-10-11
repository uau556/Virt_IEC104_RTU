/*
 * Created by SharpDevelop.
 * User: uau556
 * Date: 4-12-2017
 * Time: 9:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using lib60870;
using System.Net.Sockets;
using System.Threading;




namespace Test_RTU
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        bool running = true;
        bool mFloat = false;


        Server server = new Server();
        public ASDU newAsdu;

        private delegate void myUICallBack(string myStr, TextBox ctl);

        private void myUI(string myStr, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack myUpdate = new myUICallBack(myUI);
                this.Invoke(myUpdate, myStr, ctl);
            }
            else
            {
                ctl.AppendText(myStr + Environment.NewLine);
            }
        }

        private delegate void myCBCallBack(bool myBool, CheckBox cbox);

        private void myCB(bool myBool, CheckBox cbox)
        {
            if (this.InvokeRequired)
            {
                myCBCallBack myUpdate = new myCBCallBack(myCB);
                this.Invoke(myUpdate, myBool, cbox);
            }
            else
            {
                cbox.Checked = myBool;
            }
        }




        private void Send_Spon_Status()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECStatus);
            worker.RunWorkerAsync();
        }

        private void Send_Spon_Tap_BCD()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECTapBCD);
            worker.RunWorkerAsync();
        }

        private void Send_Spon_Tap_Binairy()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECTapBinairy);
            worker.RunWorkerAsync();
        }


        public MainForm()
        {
            
            InitializeComponent();
        }



        //IEC104 procedures
        public bool interrogationHandler(object parameter, ServerConnection connection, ASDU asdu, byte qoi)
        {

            
            
            
            //BitArray myBA = new BitArray(BitConverter.GetBytes(parameter).ToArray());
            //Console.WriteLine ("Interrogation for group " + qoi);

            if (qoi == 20) { /* Station interrogation */

                connection.SendACT_CON(asdu, false);

                ConnectionParameters cp = connection.GetConnectionParameters();

                // The CS101 allows only information object without timestamps in GI responses!

                // send information objects
                ASDU newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new MeasuredValueScaled(100, Int32.Parse(textBox2.Text), new QualityDescriptor()));
                newAsdu.AddInformationObject(new MeasuredValueScaled(101, Int32.Parse(textBox3.Text), new QualityDescriptor()));
                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new MeasuredValueShort(102, float.Parse(textBox7.Text), new QualityDescriptor()));
                newAsdu.AddInformationObject(new MeasuredValueShort(103, float.Parse(textBox6.Text), new QualityDescriptor()));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);


                newAsdu.AddInformationObject(new MeasuredValueShort(300, Int32.Parse(textBox16.Text), new QualityDescriptor()));
                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new MeasuredValueNormalized(104, Int32.Parse(textBox10.Text), new QualityDescriptor()));
                newAsdu.AddInformationObject(new MeasuredValueNormalized(105, Int32.Parse(textBox9.Text), new QualityDescriptor()));
                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new SinglePointInformation(1, checkBox2.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(2, checkBox3.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(3, checkBox4.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(4, checkBox5.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(5, checkBox6.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(6, checkBox7.Checked, new QualityDescriptor()));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                newAsdu.AddInformationObject(new SinglePointInformation(210, checkBox24.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(211, checkBox23.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(212, checkBox22.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(213, checkBox21.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(214, checkBox20.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(215, checkBox19.Checked, new QualityDescriptor()));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                int ioa = 219;

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox30.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox29.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox28.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox27.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox26.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox25.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox37.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox36.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox35.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox34.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox33.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox32.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox43.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox42.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox41.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox40.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox39.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox38.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox49.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox48.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox47.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox46.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox45.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox44.Checked, new QualityDescriptor()));


                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                if (checkBox16.Checked == true & checkBox15.Checked == false)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(10, DoublePointValue.OFF, new QualityDescriptor()));
                }
                else if (checkBox16.Checked == false & checkBox15.Checked == true)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(10, DoublePointValue.ON, new QualityDescriptor()));
                }
                else if (checkBox16.Checked == false & checkBox15.Checked == false)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(10, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                }
                else if (checkBox16.Checked == true & checkBox15.Checked == true)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(10, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                }


                if (checkBox12.Checked == true & checkBox13.Checked == false)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.OFF, new QualityDescriptor()));
                }
                else if (checkBox12.Checked == false & checkBox13.Checked == true)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.ON, new QualityDescriptor()));
                }
                else if (checkBox12.Checked == false & checkBox13.Checked == false)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                }
                else if (checkBox12.Checked == true & checkBox13.Checked == true)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                }
          
                connection.SendASDU(newAsdu);

                    newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                    newAsdu.AddInformationObject(new StepPositionInformation(200, Int32.Parse(textBox12.Text), false, new QualityDescriptor()));
                    connection.SendASDU(newAsdu);

                    connection.SendACT_TERM(asdu);
                } else {
                    connection.SendACT_CON(asdu, true);
                }

            myUI("Received GI from scada", iec104Box);

            return true;
        }





        public bool asduHandler(object parameter, ServerConnection connection, ASDU asdu)
        {

            //C_SC_NA_1   45 Single command
            //C_DC_NA_1   46 Double command
            //C_RC_NA_1   47 Regulating step command
            //C_SE_NA_1   48 Setpoint Command, normalised value
            //C_SE_NB_1   49 Setpoint Command, scaled value
            //C_SE_NC_1	  50 Setpoint Command, short floating point number

            if (asdu.TypeId == TypeID.C_SC_NA_1)
            {
                Console.WriteLine("Single command");

                SingleCommand sc = (SingleCommand)asdu.GetElement(0);

                if (sc.Select)
                {
                    myCB(true, checkBox52);
                }
                else
                {
                    myCB(false, checkBox52);
                }


                if (sc.State)
                {
                    myCB(true, checkBox51);
                    myCB(true, checkBox50);


                }
                else
                {
                    myCB(false, checkBox51);
                    myCB(false, checkBox50);
                }

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                newAsdu.AddInformationObject(new SinglePointInformation(41, checkBox50.Checked, new QualityDescriptor()));
                connection.SendASDU(newAsdu);


                Console.WriteLine(sc.ToString());
                myUI("Received Single command " + sc.ToString(), iec104Box);

            }
            else if (asdu.TypeId == TypeID.C_DC_NA_1)
            {
                Console.WriteLine("Double command");
                

                DoubleCommand  dc = (DoubleCommand)asdu.GetElement(0);

                Console.WriteLine(dc.State.ToString());
                myUI("Received Double command " + dc.State.ToString(), iec104Box);

            }
            else if (asdu.TypeId == TypeID.C_CS_NA_1)
            {

                ClockSynchronizationCommand qsc = (ClockSynchronizationCommand)asdu.GetElement(0);

                Console.WriteLine("Received clock sync command with time " + qsc.NewTime.ToString());
                myUI("Received clock sync command with time " + qsc.NewTime.ToString(), iec104Box);
            }
            else if (asdu.TypeId == TypeID.C_SE_NA_1)
            {




                Console.WriteLine("Received Setpoint normalised value");
                myUI("Received Setpoint normalised value", iec104Box);
            }
            else if (asdu.TypeId == TypeID.C_SE_NB_1)
            {




                Console.WriteLine("Received Setpoint scaled value");
                myUI("Received Setpoint scaled value", iec104Box);
            }
            else if (asdu.TypeId == TypeID.C_SE_NC_1)
            {

                


                Console.WriteLine("Received Setpoint short floating");
                myUI("Received Setpoint short floating", iec104Box);
            }

            return true;
        }



       


        private void MainForm_Load(object sender, EventArgs e)
        {

            checkBox1.Checked = true;

            //IEC104 setting
            server.DebugOutput = true;
            server.MaxQueueSize = 10;
            server.SetInterrogationHandler(interrogationHandler, null);
            server.SetASDUHandler(asduHandler, null);
            server.SetLocalPort(2404);
            //server.SetLocalAddress("10.10.10.7");
            server.Start();
        }

        private void SendIECStatus(object sender, DoWorkEventArgs e)
        {

            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=1 : CA=1 VALUE=" + checkBox2.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=2 : CA=1 VALUE=" + checkBox3.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=3 : CA=1 VALUE=" + checkBox4.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=4 : CA=1 VALUE=" + checkBox5.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=5 : CA=1 VALUE=" + checkBox6.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=6 : CA=1 VALUE=" + checkBox7.Checked.ToString(), iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new SinglePointInformation(1, checkBox2.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(2, checkBox3.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(3, checkBox4.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(4, checkBox5.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(5, checkBox6.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(6, checkBox7.Checked, new QualityDescriptor()));
            server.EnqueueASDU(newAsdu);

            while (checkBox8.Checked) {
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=1 : CA=1 VALUE=" + checkBox2.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=2 : CA=1 VALUE=" + checkBox3.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=3 : CA=1 VALUE=" + checkBox4.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=4 : CA=1 VALUE=" + checkBox5.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=5 : CA=1 VALUE=" + checkBox6.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=6 : CA=1 VALUE=" + checkBox7.Checked.ToString(), iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                newAsdu.AddInformationObject(new SinglePointInformation(1, checkBox2.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(2, checkBox3.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(3, checkBox4.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(4, checkBox5.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(5, checkBox6.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(6, checkBox7.Checked, new QualityDescriptor()));
                myUI("------------------------------------------------------------------------------------------", iec104Box);

                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox1.Text));
            }
        }



        private void SendIECTapBCD(object sender, DoWorkEventArgs e)
        {

            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=210 : CA=1 VALUE=" + checkBox24.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=211 : CA=1 VALUE=" + checkBox23.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=212 : CA=1 VALUE=" + checkBox22.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=213 : CA=1 VALUE=" + checkBox21.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=214 : CA=1 VALUE=" + checkBox20.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=215 : CA=1 VALUE=" + checkBox19.Checked.ToString(), iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new SinglePointInformation(210, checkBox24.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(211, checkBox23.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(212, checkBox22.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(213, checkBox21.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(214, checkBox20.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(215, checkBox19.Checked, new QualityDescriptor()));
            server.EnqueueASDU(newAsdu);

            while (checkBox18.Checked)
            {
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=210 : CA=1 VALUE=" + checkBox24.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=211 : CA=1 VALUE=" + checkBox23.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=212 : CA=1 VALUE=" + checkBox22.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=213 : CA=1 VALUE=" + checkBox21.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=214 : CA=1 VALUE=" + checkBox20.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=215 : CA=1 VALUE=" + checkBox19.Checked.ToString(), iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                newAsdu.AddInformationObject(new SinglePointInformation(210, checkBox24.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(211, checkBox23.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(212, checkBox22.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(213, checkBox21.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(214, checkBox20.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(215, checkBox19.Checked, new QualityDescriptor()));
                myUI("------------------------------------------------------------------------------------------", iec104Box);

                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox14.Text));
            }
        }

        private void SendIECTapBinairy(object sender, DoWorkEventArgs e)
        {

            myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=220 till 244 : CA=1", iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            int ioa = 219;

            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox30.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox29.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox28.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox27.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox26.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox25.Checked, new QualityDescriptor()));

            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox37.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox36.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox35.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox34.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox33.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox32.Checked, new QualityDescriptor()));

            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox43.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox42.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox41.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox40.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox39.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox38.Checked, new QualityDescriptor()));

            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox49.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox48.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox47.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox46.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox45.Checked, new QualityDescriptor()));
            newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox44.Checked, new QualityDescriptor()));

            server.EnqueueASDU(newAsdu);

            while (checkBox31.Checked)
            {
                myUI("SEND: SPONTANEOUS : SinglePointInformation  : IOA=220 till 224 : CA=1", iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                ioa = 219;

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox30.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox29.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox28.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox27.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox26.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox25.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox37.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox36.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox35.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox34.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox33.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox32.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox43.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox42.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox41.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox40.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox39.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox38.Checked, new QualityDescriptor()));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox49.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox48.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox47.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox46.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox45.Checked, new QualityDescriptor()));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox44.Checked, new QualityDescriptor()));

                myUI("------------------------------------------------------------------------------------------", iec104Box);

                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox15.Text));
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            Send_Spon_Status();
        }


        void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) {
                running = true;
                myUI("Start Server", iec104Box);
            } else {
                running = false;
                myUI("Stop Server", iec104Box);
            }
        }

        void Button2Click(object sender, EventArgs e)
        {
            iec104Box.Clear();
        }
        void Button3Click(object sender, EventArgs e)
        {
            server.Stop();
            System.Windows.Forms.Application.Exit();
        }




        private void SendIECDoublePoints(object sender, DoWorkEventArgs e)
        {

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);

            if (checkBox16.Checked & !checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.ON, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
            }
            else if (!checkBox16.Checked & checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.OFF, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);

            }
            else if (checkBox16.Checked & checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);

            }
            else
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

            }

            if (checkBox12.Checked & !checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.ON, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
            }
            else if (!checkBox12.Checked & checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.OFF, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);
            }
            else if (checkBox12.Checked & checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);
            }
            else
            {
                newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

            }

            server.EnqueueASDU(newAsdu);

            while (checkBox10.Checked)
            {

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);

                if (checkBox16.Checked & !checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.ON, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
                }
                else if (!checkBox16.Checked & checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.OFF, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);

                }
                else if (checkBox16.Checked & checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);

                }
                else
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

                }

                if (checkBox12.Checked & !checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.ON, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
                }
                else if (!checkBox12.Checked & checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.OFF, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);
                }
                else if (checkBox12.Checked & checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INDETERMINATE, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);
                }
                else
                {
                    newAsdu.AddInformationObject(new DoublePointInformation(11, DoublePointValue.INTERMEDIATE, new QualityDescriptor()));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

                }
                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox5.Text));
            }
        }

        private void Send_Spon_DoublePoints()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECDoublePoints);
            worker.RunWorkerAsync();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            Send_Spon_DoublePoints();
        }

        private void SendIECMeasurements(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            int rInt = r.Next(0, 100);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(100, Int32.Parse(textBox2.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=100 : CA=1 VALUE=" + textBox2.Text, iec104Box);

            rInt = r.Next(0, 100);
            newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(101, Int32.Parse(textBox3.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=101 : CA=1 VALUE=" + textBox3.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox9.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.Next(0, 100);
                newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(100, rInt, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=100 : CA=1 VALUE=" + rInt.ToString(), iec104Box);
                rInt = r.Next(0, 100);
                newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(101, rInt, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=101 : CA=1 VALUE=" + rInt.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox4.Text));
            }

        }

        private void Send_Spon_MeasurementsScaled()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECMeasurements);
            worker.RunWorkerAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Send_Spon_MeasurementsScaled();
        }

        private void SendIECMeasurementsNormalized(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            int rInt = r.Next(0, 32767);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(102, float.Parse(textBox7.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + textBox7.Text, iec104Box);

            rInt = r.Next(0, 32767);
            newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(103, float.Parse(textBox6.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=103 : CA=1 VALUE=" + textBox6.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox11.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.Next(0, 32767);
                newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(102, rInt, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + rInt.ToString(), iec104Box);
                rInt = r.Next(0, 32767);
                newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(103, rInt, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + rInt.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox4.Text));
            }
        }

        private void Send_Spon_MeasurementsNormalized()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECMeasurementsNormalized);
            worker.RunWorkerAsync();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Send_Spon_MeasurementsNormalized();
        }

        private void SendIECMeasurementsFloats(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            double rInt = r.NextDouble();

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(104, float.Parse(textBox10.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=104 : CA=1 VALUE=" + textBox10.Text, iec104Box);

            rInt = r.NextDouble();
            newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(105, float.Parse(textBox9.Text), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=105 : CA=1 VALUE=" + textBox9.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox14.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.NextDouble();
                newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(104,  float.Parse( rInt.ToString()), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=104 : CA=1 VALUE=" + rInt.ToString(), iec104Box);
                rInt = r.NextDouble();
                newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(105, float.Parse(rInt.ToString()), new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=105 : CA=1 VALUE=" + rInt.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox4.Text));
            }
        }

        private void Send_Spon_MeasurementsFloats()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECMeasurementsFloats);
            worker.RunWorkerAsync();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Send_Spon_MeasurementsFloats();
        }

        private void SendIECTapPos(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            int rInt = r.Next(0,30);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(200, Int32.Parse( textBox12.Text), false, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : StepPositionWithCP56Time2a  : IOA=200 : CA=1 VALUE=" + textBox10.Text, iec104Box);
           server.EnqueueASDU(newAsdu);

            while (checkBox17.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.Next(0,30);
                newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(200, rInt, false, new QualityDescriptor(), new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : StepPositionWithCP56Time2a  : IOA=200 : CA=1 VALUE=" + rInt.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox4.Text));
            }
        }


        private void Send_Spon_TapPos()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(SendIECTapPos);
            worker.RunWorkerAsync();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Send_Spon_TapPos();
        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Send_Spon_Tap_BCD();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Send_Spon_Tap_Binairy();
        }

        private void label67_Click(object sender, EventArgs e)
        {

        }
    }
}
