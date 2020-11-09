using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jqs
{   
    public class CncReader : BasicLogger
    {
        private string host;

        private int port;

        private ushort pHdl;

        private bool isOpened = false;

        private Dictionary<int, string> alarmTypeMapper;

        public CncReader(string ip, int port)
        {
            alarmTypeMapper = new Dictionary<int, string>() {
                {0, "Parameter switch on (SW)"},
                {1, "Power off parameter set (PW)"},
                {2, "I/O error (IO)"},
                {3, "Foreground P/S (PS)"},
                {4, "Overtravel,External data"},
                {5, "Overheat alarm"},
                {6, "Servo alarm"},
                {8, "Data I/O error"},
                {9, "Spindle alarm"},
                {10, "Other alarm(DS)"},
                {11, "Alarm concerning Malfunction prevent functions (IE)"},
                {12, "Background P/S (BG)"},
                {13, "Syncronized error (SN)"},
                {14, "reserved"},
                {15, "External alarm message"},
                {16, "reserved"},
                {17, "reserved"},
                {18, "reserved"},
                {19, "PMC error (PC)"}
            };
            this.host = ip;
            this.port = port;
            int timeout = 20;
            short statusCode = Focas1.cnc_allclibhndl3(host, (ushort) port, timeout, out pHdl);
            if (statusCode == 15) {
                logger.Error("Error: Can't find fwlib32.dll!");
                throw new Exception("Error: Can't find fwlib32.dll!");
            } else if (statusCode == 0) {
                logger.Info("Link sucess! Target host: " + host + ":" + port);
                isOpened = true;
            } else {
                logger.Error("Link failed! Target host: " + host + ":" + port);
                throw new Exception("Link failed! Target host: " + host + ":" + port);
            }
        }

        ~CncReader() {

        }

        public void close() {
            Focas1.cnc_freelibhndl(pHdl);
        }

        public string read()
        {
            if (!isOpened) 
                throw new Exception("Must open cnc connection first!");
            // logger.Info("Target ip: " + host + "; port: " + port);
            JObject resp = new JObject();
            resp["systemtime"] = DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString();
            resp["centername"] = "加工中心";
            resp["txtip"] = host;
            resp["machinename"] = "FANUC Series 32i";
            resp["spec"] = "5505";
            resp["x_relative"] = null;
            resp["z_relative"] = null;
            resp["c_relative"] = null;
            resp["v_relative"] = null;
            resp["x_absolute"] = null;
            resp["z_absolute"] = null;
            resp["c_absolute"] = null;
            resp["v_absolute"] = null;
            resp["x_machine"] = null;
            resp["z_machine"] = null;
            resp["c_machine"] = null;
            resp["v_machine"] = null;
            resp["x_distancetogo"] = null;
            resp["z_distancetogo"] = null;
            resp["c_distancetogo"] = null;
            resp["v_distancetogo"] = null;
            resp["spindle_name"] = null;
            resp["spindlenumber"] = 1;
            resp["spindleload"] = null;
            resp["spindlespeed"] = null;
            resp["sv1_load"] = null;
            resp["sv2_load"] = null;
            resp["sv3_load"] = null;
            resp["sv4_load"] = null;
            resp["cnc_status"] = null;
            resp["runningmode"] = null;
            resp["currentrunningprogramnum"] = null;
            resp["mainprogramnumber"] = null;
            resp["currentrunningncprogram"] = null;
            resp["currenttoolgroupnum"] = null;
            resp["currenttoolnum"] = null;
            resp["svnum"] = null;
            resp["cnc_versionnum"] = "3C7B5D01";
            resp["relfeedrate"] = null;
            resp["alarm_status"] = null;
            resp["alarmnum"] = null;
            resp["alarmtype"] = null;
            resp["alarmmessage"] = null;
            resp["operatingtime"] = null;
            resp["circlingtime"] = null;
            resp["cuttingtime"] = null;
            resp["powerontime"] = null;
            resp["totalparts"] = null;
            resp["cnc_turn_on"] = "开机";
            short code;
            {
                short number = Focas1.MAX_AXIS;
                Focas1.ODBPOS pos = new Focas1.ODBPOS();
                code = Focas1.cnc_rdposition(pHdl, -1, ref number, pos);
                if (code == 0)
                {
                    resp["x_relative"] = pos.p1.rel.data * Math.Pow(10, -pos.p1.rel.dec);
                    resp["z_relative"] = pos.p2.rel.data * Math.Pow(10, -pos.p2.rel.dec);
                    resp["c_relative"] = pos.p4.rel.data * Math.Pow(10, -pos.p4.rel.dec);
                    resp["v_relative"] = pos.p5.rel.data * Math.Pow(10, -pos.p5.rel.dec);
                    resp["x_absolute"] = pos.p1.abs.data * Math.Pow(10, -pos.p1.abs.dec);
                    resp["z_absolute"] = pos.p2.abs.data * Math.Pow(10, -pos.p2.abs.dec);
                    resp["c_absolute"] = pos.p4.abs.data * Math.Pow(10, -pos.p4.abs.dec);
                    resp["v_absolute"] = pos.p5.abs.data * Math.Pow(10, -pos.p5.abs.dec);
                    resp["x_machine"] = pos.p1.mach.data * Math.Pow(10, -pos.p1.mach.dec);
                    resp["z_machine"] = pos.p2.mach.data * Math.Pow(10, -pos.p2.mach.dec);
                    resp["c_machine"] = pos.p4.mach.data * Math.Pow(10, -pos.p4.mach.dec);
                    resp["v_machine"] = pos.p5.mach.data * Math.Pow(10, -pos.p5.mach.dec);
                    resp["x_distancetogo"] = pos.p1.dist.data * Math.Pow(10, - pos.p1.dist.dec);
                    resp["z_distancetogo"] = pos.p2.dist.data * Math.Pow(10, - pos.p2.dist.dec);
                    resp["c_distancetogo"] = pos.p4.dist.data * Math.Pow(10, - pos.p4.dist.dec);
                    resp["v_distancetogo"] = pos.p5.dist.data * Math.Pow(10, - pos.p5.dist.dec);
                }
            }
            {
                short number = 32;
                Focas1.ODBEXAXISNAME spindleName = new Focas1.ODBEXAXISNAME();
                code = Focas1.cnc_exaxisname(pHdl, 1, ref number, spindleName);
                if (code == 0) 
                    resp["spindle_name"] = spindleName.axname1;
            }
            {
                Focas1.ODBSVLOAD sv = new Focas1.ODBSVLOAD();
                Focas1.ODBSPLOAD sp = new Focas1.ODBSPLOAD();
                short a = 6;
                short code0 = Focas1.cnc_rdsvmeter(pHdl, ref a, sv);
                short code1 = Focas1.cnc_rdspmeter(pHdl, 1, ref a, sp);
                if (code0 == 0) 
                {
                    resp["sv1_load"] = sv.svload1.data;
                    resp["sv2_load"] = sv.svload2.data;
                    resp["sv3_load"] = sv.svload3.data;
                    resp["sv4_load"] = sv.svload4.data;
                }
                if (code1 == 0) {
                    resp["spindleload"] = sp.spload1.spload.data;
                }
            }
            {
                Focas1.ODBACT pindle = new Focas1.ODBACT();
                code = Focas1.cnc_acts(pHdl, pindle);
                if (code == 0) 
                    resp["spindlespeed"] = pindle.data;
            }
            {
                Focas1.ODBST obst = new Focas1.ODBST();
                code = Focas1.cnc_statinfo(pHdl, obst);
                if (code == 0) 
                {
                    resp["cnc_status"] = obst.run;
                    resp["runningmode"] = obst.tmmode;
                    resp["alarm_status"] = obst.alarm;
                }
            }
            {
                Focas1.ODBDY2_1 dynadata = new Focas1.ODBDY2_1();
                Focas1.ODBPRO pro = new Focas1.ODBPRO();
                Focas1.ODBSEQ seqnum = new Focas1.ODBSEQ();
                code = Focas1.cnc_rdprgnum(pHdl, pro);
                if (code == 0) 
                {
                    resp["currentrunningprogramnum"] = pro.data;
                    resp["mainprogramnumber"] = pro.mdata;
                }
                code = Focas1.cnc_rdseqnum(pHdl, seqnum);
                if (code == 0) 
                    resp["currentrunningncprogram"] = seqnum.data;
            }
            {
                Focas1.ODBTG btg = new Focas1.ODBTG();
                Focas1.ODBUSEGRP grp = new Focas1.ODBUSEGRP();
                code = Focas1.cnc_rdtlusegrp(pHdl, grp);
                short a = Convert.ToInt16(grp.use);
                code = Focas1.cnc_rdtoolgrp(pHdl, a, 20 + 20 * 1, btg);
                resp["currenttoolgroupnum"] = a;
                resp["currenttoolnum"] = btg.data.data1.tool_num; 
            }
            {
    			Focas1.ODBSYS odbsys = new Focas1.ODBSYS();
                code = Focas1.cnc_sysinfo(pHdl, odbsys);
                if (code == 0)
                    resp["svnum"] = odbsys.axes[1];
            }
            {
                Focas1.ODBACT odbact = new Focas1.ODBACT();
			    code = Focas1.cnc_actf(pHdl, odbact);
                if (code == 0)
                    resp["relfeedrate"] = odbact.data;
            }
            {
                short b = 8;
                Focas1.ODBALMMSG2 msg = new Focas1.ODBALMMSG2();
                code = Focas1.cnc_rdalmmsg2(pHdl, -1, ref b, msg);
                if (code == 0)
                {
                    resp["alarmnum"] = msg.msg2.alm_no;
                    resp["alarmmessage"] = msg.msg2.alm_msg;
                }
            }
            {
                int alarm = 0;
                code = Focas1.cnc_alarm2(pHdl, out alarm);
                if (code == 0)
                    resp["alarmtype"] = alarmTypeMapper.ContainsKey(alarm) ? alarmTypeMapper[alarm] : null;
            }
            {
                Focas1.IODBPSD_1 param6751 = new Focas1.IODBPSD_1();
                Focas1.IODBPSD_1 param6752 = new Focas1.IODBPSD_1();
                code = Focas1.cnc_rdparam(pHdl, 6751, 0, 8, param6751);
                if (code == 0) 
                {
                    int workingTimeSec = param6751.ldata / 1000;
                    code = Focas1.cnc_rdparam(pHdl, 6752, 0, 8, param6752);
                    if(code==0) 
                    {
                        int workingTimeMin = param6752.ldata;
                        int workingTime= workingTimeMin * 60 + workingTimeSec;
                        resp["operatingtime"] = workingTime;
                    }
                }
            }
            {
                Focas1.IODBPSD_1 param6757 = new Focas1.IODBPSD_1();
                Focas1.IODBPSD_1 param6758 = new Focas1.IODBPSD_1();
                code = Focas1.cnc_rdparam(pHdl, 6757, 0, 8, param6757);
                if (code == 0) {
                    int circlingTimeSec = param6757.ldata / 1000;
                    code = Focas1.cnc_rdparam(pHdl, 6758, 0, 8, param6758);
                    if (code == 0) {
                        int circlingTimeMin = param6758.ldata;
                        int workingTime = circlingTimeMin * 60 + circlingTimeSec;
                        resp["circlingtime"] = workingTime;
                    }

                }
            }
            {
                Focas1.IODBPSD_1 param6753 = new Focas1.IODBPSD_1();
                Focas1.IODBPSD_1 param6754 = new Focas1.IODBPSD_1();
                code = Focas1.cnc_rdparam(pHdl, 6753, 0, 8 + 32, param6753);
                if (code == 0)
                {
                    int cuttingTimeSec = param6753.ldata / 1000;
                    code = Focas1.cnc_rdparam(pHdl, 6754, 0, 8 + 32, param6754);
                    if (code == 0)
                    {
                        int cuttingTimeMin = param6754.ldata;
                        int workingTime = cuttingTimeMin * 60 + cuttingTimeSec;
                        resp["cuttingtime"] = workingTime;
                    }
                }
            }
            {
                Focas1.IODBPSD_1 param6750 = new Focas1.IODBPSD_1();
                code = Focas1.cnc_rdparam(pHdl, 6750, 0, 8 + 32, param6750);
                if (code==0)
                {
                    int powerOnTime= param6750.ldata * 60;
                    resp["powerontime"] = powerOnTime;
                }
            }
            {
                Focas1.IODBPSD_1 param6712 = new Focas1.IODBPSD_1();
                code = Focas1.cnc_rdparam(pHdl, 6712, 0, 8, param6712);
                if (code == 0)
                {
                    resp["totalparts"] = param6712.ldata;
                }
            }
            return resp.ToString();
        }
    }
}