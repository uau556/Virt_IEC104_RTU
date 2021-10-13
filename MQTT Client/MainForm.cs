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


        static float NextFloat(float min, float max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

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


        private delegate void myTBCallBack(string myStr, TextBox tbox);

        private void myTB(string myStr, TextBox tbox)
        {
            if (this.InvokeRequired)
            {
                myTBCallBack myUpdate = new myTBCallBack(myTB);
                this.Invoke(myUpdate, myStr, tbox);
            }
            else
            {
                tbox.Text = myStr;
            }
        }

        private delegate void myLBCallBack(string myStr, Label lb);

        private void myLB(string myStr, Label lb)
        {
            if (this.InvokeRequired)
            {
                myLBCallBack myUpdate = new myLBCallBack(myLB);
                this.Invoke(myUpdate, myStr, lb);
            }
            else
            {
                lb.Text = myStr;
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
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            if (qoi == 20)
            { /* Station interrogation */

                connection.SendACT_CON(asdu, false);

                ConnectionParameters cp = connection.GetConnectionParameters();

                // The CS101 allows only information object without timestamps in GI responses!

                // send information objects
                ASDU newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                //newAsdu.AddInformationObject(new MeasuredValueScaled(100, -32000, QD));
                //newAsdu.AddInformationObject(new MeasuredValueScaled(101, 32000, QD));
                //connection.SendASDU(newAsdu);

                //newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new MeasuredValueNormalized(102, float.Parse(textBox7.Text), QD));
                newAsdu.AddInformationObject(new MeasuredValueNormalized(103, float.Parse(textBox6.Text), QD));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);


                newAsdu.AddInformationObject(new MeasuredValueShort(300, float.Parse(textBox16.Text), QD));
                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new MeasuredValueShort(104, float.Parse(textBox10.Text), QD));
                newAsdu.AddInformationObject(new MeasuredValueShort(105, float.Parse(textBox9.Text), QD));
                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                newAsdu.AddInformationObject(new SinglePointInformation(1, checkBox2.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(2, checkBox3.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(3, checkBox4.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(4, checkBox5.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(5, checkBox6.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(6, checkBox7.Checked, QD));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                newAsdu.AddInformationObject(new SinglePointInformation(210, checkBox24.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(211, checkBox23.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(212, checkBox22.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(213, checkBox21.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(214, checkBox20.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(215, checkBox19.Checked, QD));

                connection.SendASDU(newAsdu);
                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                int ioa = 219;

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox30.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox29.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox28.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox27.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox26.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox25.Checked, QD));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox37.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox36.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox35.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox34.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox33.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox32.Checked, QD));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox43.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox42.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox41.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox40.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox39.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox38.Checked, QD));

                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox49.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox48.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox47.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox46.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox45.Checked, QD));
                newAsdu.AddInformationObject(new SinglePointInformation(ioa++, checkBox44.Checked, QD));


                connection.SendASDU(newAsdu);

                newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);

                DoublePointValue dpv = DoublePointValue.OFF;

                if (checkBox16.Checked == true & checkBox15.Checked == false)
                {
                    dpv = DoublePointValue.OFF;
                }
                else if (checkBox16.Checked == false & checkBox15.Checked == true)
                {
                    dpv = DoublePointValue.ON;
                }
                else if (checkBox16.Checked == false & checkBox15.Checked == false)
                {
                    dpv = DoublePointValue.INTERMEDIATE;
                }
                else if (checkBox16.Checked == true & checkBox15.Checked == true)
                {
                    dpv = DoublePointValue.INDETERMINATE;
                }
                newAsdu.AddInformationObject(new DoublePointInformation(10, dpv, QD));


                if (checkBox12.Checked == true & checkBox13.Checked == false)
                {
                    dpv = DoublePointValue.OFF;
                }
                else if (checkBox12.Checked == false & checkBox13.Checked == true)
                {
                    dpv = DoublePointValue.ON;
                }
                else if (checkBox12.Checked == false & checkBox13.Checked == false)
                {
                    dpv = DoublePointValue.INTERMEDIATE;
                }
                else if (checkBox12.Checked == true & checkBox13.Checked == true)
                {
                    dpv = DoublePointValue.INDETERMINATE;
                }
                newAsdu.AddInformationObject(new DoublePointInformation(11, dpv, QD));

                connection.SendASDU(newAsdu);

                    newAsdu = new ASDU(cp, CauseOfTransmission.INTERROGATED_BY_STATION, false, false, 2, 1, true);
                    newAsdu.AddInformationObject(new StepPositionInformation(200, Int32.Parse(textBox12.Text), false, QD));
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

            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();


            if (asdu.TypeId == TypeID.C_SC_NA_1)
            {
                Console.WriteLine("Recieved Single command");

                SingleCommand sc = (SingleCommand)asdu.GetElement(0);

                if (sc.ObjectAddress == 40)
                {
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

                    Thread.Sleep(Int32.Parse(textBox2.Text));
                    if (checkBox57.Checked == false)
                    {
                        newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                        newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(41, checkBox50.Checked, QD, new CP56Time2a(DateTime.Now)));
                        connection.SendASDU(newAsdu);
                    }
                    else
                    {
                        myUI("Send No Activation Conformation", iec104Box);
                    }

                    myUI("Received Single command " + sc.ToString() + " " + sc.ObjectAddress.ToString(), iec104Box);
                }
                else
                {
                    myUI("Received Single command with wrong IOA", iec104Box);
                }

            }
            else if (asdu.TypeId == TypeID.C_DC_NA_1)
            {
                Console.WriteLine("Double command");

                DoubleCommand dc = (DoubleCommand)asdu.GetElement(0);

                if (dc.ObjectAddress == 50)
                {
                    if (dc.Select)
                    {
                        myCB(false, checkBox55);
                    }
                    else
                    {
                        myCB(true, checkBox55);
                    }

                    DoublePointValue dpv;

                    if (dc.State == 1)
                    {
                        myCB(false, checkBox54);
                        myCB(true, checkBox53);

                        myCB(false, checkBox56);
                        myCB(true, checkBox9);

                        dpv = DoublePointValue.OFF;
                    }
                    else
                    {
                        myCB(true, checkBox54);
                        myCB(false, checkBox53);

                        myCB(true, checkBox56);
                        myCB(false, checkBox9);

                        dpv = DoublePointValue.ON;
                    }

                    Thread.Sleep(Int32.Parse(textBox2.Text));
                    if (checkBox57.Checked == false)
                    {
                        newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                        newAsdu.AddInformationObject(new DoublePointInformation(51, dpv, QD));
                        connection.SendASDU(newAsdu);
                    }
                    else
                    {
                        myUI("Send No Activation Conformation", iec104Box);
                    }

                    Console.WriteLine(dc.State.ToString());
                    myUI("Received Double command " + dc.State.ToString() + " " + dc.ObjectAddress.ToString(), iec104Box);
                }
                else
                {
                    myUI("Received Double command with wrong IOA", iec104Box);
                }
            }
            else if (asdu.TypeId == TypeID.C_CS_NA_1)
            {

                ClockSynchronizationCommand qsc = (ClockSynchronizationCommand)asdu.GetElement(0);

                Console.WriteLine("Received clock sync command with time " + qsc.NewTime.ToString());
                myUI("Received clock sync command with time " + qsc.NewTime.ToString(), iec104Box);
            }
            else if (asdu.TypeId == TypeID.C_SE_NA_1)
            {
                SetpointCommandNormalized spc = (SetpointCommandNormalized)asdu.GetElement(0);

                if (spc.ObjectAddress == 400)
                {

                    Console.WriteLine(spc.NormalizedValue);
                    Console.WriteLine(spc.RawValue);

                    myTB(spc.NormalizedValue.ToString(), textBox19);
                    myTB(spc.NormalizedValue.ToString(), textBox16);

                    Thread.Sleep(Int32.Parse(textBox2.Text));
                    if (checkBox57.Checked == false)
                    {
                        newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                        newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(401, float.Parse(textBox16.Text), QD, new CP56Time2a(DateTime.Now)));
                        server.EnqueueASDU(newAsdu);
                    }
                    else
                    {
                        myUI("Send No Activation Conformation", iec104Box);
                    }



                    Console.WriteLine("Received SetpointCommandNormalized");
                    myUI("Received SetpointCommandNormalized", iec104Box);
                }
                else
                {
                    myUI("Received SetpointCommandNormalized with wrong IOA", iec104Box);
                }
            }
            else if (asdu.TypeId == TypeID.C_SE_NB_1)
            {
                Console.WriteLine("Received Setpoint scaled value");
                myUI("Received Setpoint scaled value", iec104Box);
            }
            else if (asdu.TypeId == TypeID.C_SE_NC_1)
            {
                SetpointCommandShort sps = (SetpointCommandShort)asdu.GetElement(0);

                if (sps.ObjectAddress == 500)
                {

                    Console.WriteLine(sps.Value);

                    myTB(sps.Value.ToString(), textBox21);
                    myTB(sps.Value.ToString(), textBox20);

                    Thread.Sleep(Int32.Parse(textBox2.Text));
                    if (checkBox57.Checked == false)
                    {

                        newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                        newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(501, float.Parse(textBox20.Text), QD, new CP56Time2a(DateTime.Now)));
                        server.EnqueueASDU(newAsdu);
                    }
                    else
                    {
                        myUI("Send No Activation Conformation", iec104Box);
                    }

                    Console.WriteLine("Received SetpointCommandShort (Float)");
                    myUI("Received SetpointCommandShort (Float)", iec104Box);
                }
                else
                {
                    myUI("Received SetpointCommandShort (Float) with wrong IOA", iec104Box);
                }

            }
            else if (asdu.TypeId == TypeID.C_RC_NA_1)
            {
                int sp = 0;
                StepCommand stepCommand = (StepCommand)asdu.GetElement(0);
                if (stepCommand.ObjectAddress == 600)
                {

                    Console.WriteLine(stepCommand.ToString());

                    myLB(stepCommand.State.ToString(), label89);

                    sp = Int32.Parse(textBox12.Text);

                    if (radioButton1.Checked)
                    {
                        if(stepCommand.State == StepCommandValue.LOWER & sp > 0)
                        {
                            sp--;
                        }
                        else if (stepCommand.State == StepCommandValue.HIGHER & sp < 25)
                        {
                            sp++;
                        }

                        Thread.Sleep(Int32.Parse(textBox2.Text));
                        if (checkBox57.Checked == false)
                        {
                            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                            newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(200, sp, false, QD, new CP56Time2a(DateTime.Now)));
                            server.EnqueueASDU(newAsdu);
                        }
                        else
                        {
                            myUI("Send No Activation Conformation", iec104Box);
                        }

                        myTB(sp.ToString(), textBox12);

                    }
                    else if(radioButton2.Checked)
                    {

                        int[] bcd = { Convert.ToInt32(checkBox24.Checked), Convert.ToInt32(checkBox23.Checked), Convert.ToInt32(checkBox22.Checked), Convert.ToInt32(checkBox21.Checked), Convert.ToInt32(checkBox20.Checked), Convert.ToInt32(checkBox19.Checked) };

                        sp = BCD6ToInt(bcd);
                        Console.WriteLine(sp.ToString());

                        if (stepCommand.State == StepCommandValue.LOWER & sp > 0)
                        {
                            sp--;
                        }
                        else if (stepCommand.State == StepCommandValue.HIGHER & sp < 25)
                        {
                            sp++;
                        }

                        bcd = IntToBCD6(sp);

                        myCB(Convert.ToBoolean(bcd[0]), checkBox24);
                        myCB(Convert.ToBoolean(bcd[1]), checkBox23);
                        myCB(Convert.ToBoolean(bcd[2]), checkBox22);
                        myCB(Convert.ToBoolean(bcd[3]), checkBox21);
                        myCB(Convert.ToBoolean(bcd[4]), checkBox20);
                        myCB(Convert.ToBoolean(bcd[5]), checkBox19);


                        Thread.Sleep(Int32.Parse(textBox2.Text));
                        if (checkBox57.Checked == false)
                        {
                            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(210, checkBox24.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(211, checkBox23.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(212, checkBox22.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(213, checkBox21.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(214, checkBox20.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(215, checkBox19.Checked, QD, new CP56Time2a(DateTime.Now)));
                            server.EnqueueASDU(newAsdu);
                            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                            newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(600, Int32.Parse(textBox12.Text), false, QD, new CP56Time2a(DateTime.Now)));
                            server.EnqueueASDU(newAsdu);
                        }
                        else
                        {
                            myUI("Send No Activation Conformation", iec104Box);
                        }
                    }
                    else if(radioButton3.Checked)
                    {

                        int[] reeks = {   Convert.ToInt32(checkBox30.Checked),
                                        Convert.ToInt32(checkBox29.Checked),
                                        Convert.ToInt32(checkBox28.Checked),
                                        Convert.ToInt32(checkBox27.Checked),
                                        Convert.ToInt32(checkBox26.Checked),
                                        Convert.ToInt32(checkBox25.Checked),
                                        Convert.ToInt32(checkBox37.Checked),
                                        Convert.ToInt32(checkBox36.Checked),
                                        Convert.ToInt32(checkBox35.Checked),
                                        Convert.ToInt32(checkBox34.Checked),
                                        Convert.ToInt32(checkBox33.Checked),
                                        Convert.ToInt32(checkBox32.Checked),
                                        Convert.ToInt32(checkBox43.Checked),
                                        Convert.ToInt32(checkBox42.Checked),
                                        Convert.ToInt32(checkBox41.Checked),
                                        Convert.ToInt32(checkBox40.Checked),
                                        Convert.ToInt32(checkBox39.Checked),
                                        Convert.ToInt32(checkBox38.Checked),
                                        Convert.ToInt32(checkBox49.Checked),
                                        Convert.ToInt32(checkBox48.Checked),
                                        Convert.ToInt32(checkBox47.Checked),
                                        Convert.ToInt32(checkBox46.Checked),
                                        Convert.ToInt32(checkBox45.Checked),
                                        Convert.ToInt32(checkBox44.Checked)
                        };

                        sp = ByteToInt(reeks);
                        Console.WriteLine(sp.ToString());

                        if (stepCommand.State == StepCommandValue.LOWER & sp > 0)
                        {
                            sp--;
                        }
                        else if (stepCommand.State == StepCommandValue.HIGHER & sp < 24)
                        {
                            sp++;
                        }

                        reeks = IntToByte(sp);

                        myCB(Convert.ToBoolean(reeks[0]),  checkBox30);
                        myCB(Convert.ToBoolean(reeks[1]),  checkBox29);
                        myCB(Convert.ToBoolean(reeks[2]),  checkBox28);
                        myCB(Convert.ToBoolean(reeks[3]),  checkBox27);
                        myCB(Convert.ToBoolean(reeks[4]),  checkBox26);
                        myCB(Convert.ToBoolean(reeks[5]),  checkBox25);
                        myCB(Convert.ToBoolean(reeks[6]),  checkBox37);
                        myCB(Convert.ToBoolean(reeks[7]),  checkBox36);
                        myCB(Convert.ToBoolean(reeks[8]),  checkBox35);
                        myCB(Convert.ToBoolean(reeks[9]),  checkBox34);
                        myCB(Convert.ToBoolean(reeks[10]), checkBox33);
                        myCB(Convert.ToBoolean(reeks[11]), checkBox32);
                        myCB(Convert.ToBoolean(reeks[12]), checkBox43);
                        myCB(Convert.ToBoolean(reeks[13]), checkBox42);
                        myCB(Convert.ToBoolean(reeks[14]), checkBox41);
                        myCB(Convert.ToBoolean(reeks[15]), checkBox40);
                        myCB(Convert.ToBoolean(reeks[16]), checkBox39);
                        myCB(Convert.ToBoolean(reeks[17]), checkBox38);
                        myCB(Convert.ToBoolean(reeks[18]), checkBox49);
                        myCB(Convert.ToBoolean(reeks[19]), checkBox48);
                        myCB(Convert.ToBoolean(reeks[20]), checkBox47);
                        myCB(Convert.ToBoolean(reeks[21]), checkBox46);
                        myCB(Convert.ToBoolean(reeks[22]), checkBox45);
                        myCB(Convert.ToBoolean(reeks[23]), checkBox44);


                        Thread.Sleep(Int32.Parse(textBox2.Text));
                        if (checkBox57.Checked == false)
                        {
                            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);

                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(220, checkBox30.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(221, checkBox29.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(222, checkBox28.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(223, checkBox27.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(224, checkBox26.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(225, checkBox25.Checked, QD, new CP56Time2a(DateTime.Now)));

                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(226, checkBox37.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(227, checkBox36.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(228, checkBox35.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(229, checkBox34.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(230, checkBox33.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(231, checkBox32.Checked, QD, new CP56Time2a(DateTime.Now)));

                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(232, checkBox43.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(233, checkBox42.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(234, checkBox41.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(235, checkBox40.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(236, checkBox39.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(237, checkBox38.Checked, QD, new CP56Time2a(DateTime.Now)));

                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(238, checkBox49.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(239, checkBox48.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(240, checkBox47.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(241, checkBox46.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(242, checkBox45.Checked, QD, new CP56Time2a(DateTime.Now)));
                            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(243, checkBox44.Checked, QD, new CP56Time2a(DateTime.Now)));

                            server.EnqueueASDU(newAsdu);
                            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.ACTIVATION_CON, false, false, 2, 1, false);
                            newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(600, Int32.Parse(textBox12.Text), false, QD, new CP56Time2a(DateTime.Now)));
                            server.EnqueueASDU(newAsdu);
                        }
                        else
                        {
                            myUI("Send No Activation Conformation", iec104Box);
                        }

                    }

                    Console.WriteLine("Received StepCommand ");
                    myUI("Received StepCommand", iec104Box);
                }
                else
                {
                    myUI("Received StepCommand with wrong IOA", iec104Box);
                }
            }
            return true;
        }

        public int ByteToInt(int[] bcd)
        {
            int outInt = 0;

            outInt =    bcd[0]  *  1 + bcd[1]  * 2  + bcd[2]  * 3  + bcd[3]  * 4  + bcd[4]  * 5  + bcd[5]  * 6  + //
                        bcd[6]  *  7 + bcd[7]  * 8  + bcd[8]  * 9  + bcd[9]  * 10 + bcd[10] * 11 + bcd[11] * 12 + //
                        bcd[12] * 13 + bcd[13] * 14 + bcd[14] * 15 + bcd[15] * 16 + bcd[16] * 17 + bcd[17] * 18 + //
                        bcd[18] * 19 + bcd[19] * 20 + bcd[20] * 21 + bcd[21] * 22 + bcd[22] * 23 + bcd[23] * 24 ;

            return outInt;
        }

        public int[] IntToByte(int numericvalue)
        {
            int[] reeks = new int[24];

            if (numericvalue > 0 & numericvalue < 25 )
            {
                reeks[numericvalue - 1] = 1;
            }

            return reeks;
        }

        public int BCD6ToInt(int[] bcd)
        {
            int outInt = 0;

            outInt = bcd[0] * 1 + bcd[1] * 2 + bcd[2] * 4 + bcd[3] * 8 + bcd[4] * 10 + bcd[5] * 20;

            return outInt;
        }

        public int[] IntToBCD6(int numericvalue)
        {
            int[] bcd = new int[6];
            int rest = numericvalue;

            if (rest >= 20)
            {
                bcd[5] = 1;
            }
            rest = rest % 20;

            if (rest >= 10)
            {
                bcd[4] = 1;
            }
            rest = rest % 10;

            if (rest >= 8)
            {
                bcd[3] = 1;
            }
            rest = rest % 8;

            if (rest >= 4)
            {
                bcd[2] = 1;
            }
            rest = rest % 4;

            if (rest >= 2)
            {
                bcd[1] = 1;
            }
            rest = rest % 2;

            bcd[0] = rest % 1;
            if (rest ==1)
            {
                bcd[0] = 1;
            }

            return bcd;
        }


        public QualityDescriptor setQuality( )
        {
            QualityDescriptor QD = new QualityDescriptor();
     
            if (checkBox58.Checked) QD.Invalid      = true; else QD.Invalid     = false;
            if (checkBox59.Checked) QD.NonTopical   = true; else QD.NonTopical  = false;
            if (checkBox61.Checked) QD.Blocked      = true; else QD.Blocked     = false;
            if (checkBox60.Checked) QD.Substituted  = true; else QD.Substituted = false;
            if (checkBox62.Checked) QD.Overflow     = true; else QD.Overflow    = false;

            return QD;
        }



        private void MainForm_Load(object sender, EventArgs e)
        {



                       
            checkBox1.Checked = true;
            panel6.Enabled = true;
            panel7.Enabled = false;
            panel8.Enabled = false;


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
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=1 : CA=1 VALUE=" + checkBox2.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=2 : CA=1 VALUE=" + checkBox3.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=3 : CA=1 VALUE=" + checkBox4.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=4 : CA=1 VALUE=" + checkBox5.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=5 : CA=1 VALUE=" + checkBox6.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=6 : CA=1 VALUE=" + checkBox7.Checked.ToString(), iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(1, checkBox2.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(2, checkBox3.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(3, checkBox4.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(4, checkBox5.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(5, checkBox6.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(6, checkBox7.Checked, QD, new CP56Time2a(DateTime.Now)));
            server.EnqueueASDU(newAsdu);

            while (checkBox8.Checked) {
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=1 : CA=1 VALUE=" + checkBox2.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=2 : CA=1 VALUE=" + checkBox3.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=3 : CA=1 VALUE=" + checkBox4.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=4 : CA=1 VALUE=" + checkBox5.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=5 : CA=1 VALUE=" + checkBox6.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=6 : CA=1 VALUE=" + checkBox7.Checked.ToString(), iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(1, checkBox2.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(2, checkBox3.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(3, checkBox4.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(4, checkBox5.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(5, checkBox6.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(6, checkBox7.Checked, QD, new CP56Time2a(DateTime.Now)));
                myUI("------------------------------------------------------------------------------------------", iec104Box);

                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox1.Text));
            }
        }



        private void SendIECTapBCD(object sender, DoWorkEventArgs e)
        {
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=210 : CA=1 VALUE=" + checkBox24.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=211 : CA=1 VALUE=" + checkBox23.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=212 : CA=1 VALUE=" + checkBox22.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=213 : CA=1 VALUE=" + checkBox21.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=214 : CA=1 VALUE=" + checkBox20.Checked.ToString(), iec104Box);
            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=215 : CA=1 VALUE=" + checkBox19.Checked.ToString(), iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(210, checkBox24.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(211, checkBox23.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(212, checkBox22.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(213, checkBox21.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(214, checkBox20.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(215, checkBox19.Checked, QD, new CP56Time2a(DateTime.Now)));
            server.EnqueueASDU(newAsdu);
            //newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(102, float.Parse(textBox7.Text), QD, new CP56Time2a(DateTime.Now)));
            while (checkBox18.Checked)
            {
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=210 : CA=1 VALUE=" + checkBox24.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=211 : CA=1 VALUE=" + checkBox23.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=212 : CA=1 VALUE=" + checkBox22.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=213 : CA=1 VALUE=" + checkBox21.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=214 : CA=1 VALUE=" + checkBox20.Checked.ToString(), iec104Box);
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=215 : CA=1 VALUE=" + checkBox19.Checked.ToString(), iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(210, checkBox24.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(211, checkBox23.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(212, checkBox22.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(213, checkBox21.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(214, checkBox20.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(215, checkBox19.Checked, QD, new CP56Time2a(DateTime.Now)));
                myUI("------------------------------------------------------------------------------------------", iec104Box);

                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox14.Text));
            }
        }

        private void SendIECTapBinairy(object sender, DoWorkEventArgs e)
        {
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=220 till 244 : CA=1", iec104Box);
            myUI("------------------------------------------------------------------------------------------", iec104Box);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            int ioa = 219;

            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox30.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox29.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox28.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox27.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox26.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox25.Checked, QD, new CP56Time2a(DateTime.Now)));

            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox37.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox36.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox35.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox34.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox33.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox32.Checked, QD, new CP56Time2a(DateTime.Now)));

            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox43.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox42.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox41.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox40.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox39.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox38.Checked, QD, new CP56Time2a(DateTime.Now)));

            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox49.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox48.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox47.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox46.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox45.Checked, QD, new CP56Time2a(DateTime.Now)));
            newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox44.Checked, QD, new CP56Time2a(DateTime.Now)));

            server.EnqueueASDU(newAsdu);

            while (checkBox31.Checked)
            {
                myUI("SEND: SPONTANEOUS : SinglePointWithCP56Time2a  : IOA=220 till 224 : CA=1", iec104Box);

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                ioa = 219;

                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox30.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox29.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox28.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox27.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox26.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox25.Checked, QD, new CP56Time2a(DateTime.Now)));

                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox37.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox36.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox35.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox34.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox33.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox32.Checked, QD, new CP56Time2a(DateTime.Now)));

                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox43.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox42.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox41.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox40.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox39.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox38.Checked, QD, new CP56Time2a(DateTime.Now)));

                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox49.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox48.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox47.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox46.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox45.Checked, QD, new CP56Time2a(DateTime.Now)));
                newAsdu.AddInformationObject(new SinglePointWithCP56Time2a(ioa++, checkBox44.Checked, QD, new CP56Time2a(DateTime.Now)));

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
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);

            if (checkBox16.Checked & !checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.ON, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
            }
            else if (!checkBox16.Checked & checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.OFF, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);

            }
            else if (checkBox16.Checked & checkBox15.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.INDETERMINATE, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);

            }
            else
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.INTERMEDIATE, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

            }

            if (checkBox12.Checked & !checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.ON, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
            }
            else if (!checkBox12.Checked & checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.OFF, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);
            }
            else if (checkBox12.Checked & checkBox13.Checked)
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.INDETERMINATE, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);
            }
            else
            {
                newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.INTERMEDIATE, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

            }

            server.EnqueueASDU(newAsdu);

            while (checkBox10.Checked)
            {

                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);

                if (checkBox16.Checked & !checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.ON, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
                }
                else if (!checkBox16.Checked & checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.OFF, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);

                }
                else if (checkBox16.Checked & checkBox15.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.INDETERMINATE, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);

                }
                else
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(10, DoublePointValue.INTERMEDIATE, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=10 : CA=1 VALUE=" + DoublePointValue.INTERMEDIATE.ToString(), iec104Box);

                }

                if (checkBox12.Checked & !checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.ON, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.ON.ToString(), iec104Box);
                }
                else if (!checkBox12.Checked & checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.OFF, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.OFF.ToString(), iec104Box);
                }
                else if (checkBox12.Checked & checkBox13.Checked)
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.INDETERMINATE, QD, new CP56Time2a(DateTime.Now)));
                    myUI("SEND: SPONTANEOUS : DoublePointInformation  : IOA=11 : CA=1 VALUE=" + DoublePointValue.INDETERMINATE.ToString(), iec104Box);
                }
                else
                {
                    newAsdu.AddInformationObject(new DoublePointWithCP56Time2a(11, DoublePointValue.INTERMEDIATE, QD, new CP56Time2a(DateTime.Now)));
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

       /* private void SendIECMeasurements(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            int rInt = r.Next(0, 100);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(100, Int32.Parse(textBox2.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=100 : CA=1 VALUE=" + textBox2.Text, iec104Box);

            rInt = r.Next(0, 100);
            newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(101, Int32.Parse(textBox3.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=101 : CA=1 VALUE=" + textBox3.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox9.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.Next(0, 100);
                newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(100, rInt, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueScaledWithCP56Time2a  : IOA=100 : CA=1 VALUE=" + rInt.ToString(), iec104Box);
                rInt = r.Next(0, 100);
                newAsdu.AddInformationObject(new MeasuredValueScaledWithCP56Time2a(101, rInt, QD, new CP56Time2a(DateTime.Now)));
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
        */
        private void SendIECMeasurementsNormalized(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            double rDouble = r.NextDouble();
            float rFloat;

            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();


            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(102, float.Parse(textBox7.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS :MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + textBox7.Text, iec104Box);

            
            newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(103, float.Parse(textBox6.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=103 : CA=1 VALUE=" + textBox6.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox11.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rDouble = r.NextDouble();
                rFloat = Convert.ToSingle(rDouble);

                newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(102, rFloat, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + rFloat.ToString(), iec104Box);
                
                rDouble = r.NextDouble();
                rFloat = Convert.ToSingle(rDouble);
                newAsdu.AddInformationObject(new MeasuredValueNormalizedWithCP56Time2a(103, rFloat, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueNormalizedWithCP56Time2a  : IOA=102 : CA=1 VALUE=" + rFloat.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox8.Text));
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
            //Random r = new Random();
            //double rDouble = r.NextDouble();
            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();


            float rFloat = NextFloat(0,100);

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(104, float.Parse(textBox10.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=104 : CA=1 VALUE=" + textBox10.Text, iec104Box);


            newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(105, float.Parse(textBox9.Text), QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=105 : CA=1 VALUE=" + textBox9.Text, iec104Box);
            server.EnqueueASDU(newAsdu);

            while (checkBox14.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rFloat = NextFloat(0, 100);
                newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(104, rFloat, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=104 : CA=1 VALUE=" + rFloat.ToString(), iec104Box);
                rFloat = NextFloat(100, 1000);
                newAsdu.AddInformationObject(new MeasuredValueShortWithCP56Time2a(105, rFloat, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : MeasuredValueShortWithCP56Time2a  : IOA=105 : CA=1 VALUE=" + rFloat.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox11.Text));
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

            QualityDescriptor QD = new QualityDescriptor();
            QD = setQuality();

            newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
            newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(200, Int32.Parse( textBox12.Text), false, QD, new CP56Time2a(DateTime.Now)));
            myUI("SEND: SPONTANEOUS : StepPositionWithCP56Time2a  : IOA=200 : CA=1 VALUE=" + textBox10.Text, iec104Box);
           server.EnqueueASDU(newAsdu);

            while (checkBox17.Checked)
            {
                newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.SPONTANEOUS, false, false, 2, 1, false);
                rInt = r.Next(0,30);
                newAsdu.AddInformationObject(new StepPositionWithCP56Time2a(200, rInt, false, QD, new CP56Time2a(DateTime.Now)));
                myUI("SEND: SPONTANEOUS : StepPositionWithCP56Time2a  : IOA=200 : CA=1 VALUE=" + rInt.ToString(), iec104Box);

                myUI("------------------------------------------------------------------------------------------", iec104Box);
                server.EnqueueASDU(newAsdu);
                Thread.Sleep(Int32.Parse(textBox13.Text));
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //myUI("radioButton1_CheckedChanged", iec104Box);
            if (radioButton1.Checked)
            {
                panel6.Enabled = true;
                panel7.Enabled = false;
                panel8.Enabled = false;
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                panel6.Enabled = false;
                panel7.Enabled = true;
                panel8.Enabled = false;
            }

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                panel6.Enabled = false;
                panel7.Enabled = false;
                panel8.Enabled = true;
            }

        }

        private void label98_Click(object sender, EventArgs e)
        {

        }

        private void checkBox58_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}


