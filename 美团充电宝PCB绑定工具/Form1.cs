using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace 美团充电宝PCB绑定工具
{
    public partial class Form1 : Form
    {
        private bool IsRun = false;
        private string BindUrl = "http://192.168.10.84/mes-interface/api/sn/group-bind";
        private int BindCount = 0;
        private static SerialPort _serialPort;
        private static SerialPort PLC;
        private static readonly SemaphoreSlim PLCLock = new SemaphoreSlim(1, 1);
        private bool IsNG = false;
        private string NGCmd = "01 05 00 96 FF 00 6C 16";
        private string NGEnd = "01 05 00 96 00 00 2D E6";
        private string PASSCmd = "01 05 00 97 FF 00 3D D6";
        private string PASSEnd = "01 05 00 97 00 00 7C 26";
        private string ResetCmd = "01 05 00 9B FF 00 FD D5";

        private readonly object _lock = new object();
        public Form1()
        {
            InitializeComponent();
            Text = "美团充电宝PCB绑定工具  " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void SerialInit(string com)
        {
            // 配置串口参数
            string portName = com;       // 端口号
            int baudRate = 115200;            // 波特率
            Parity parity = Parity.None;   // 校验位
            int dataBits = 8;              // 数据位
            StopBits stopBits = StopBits.One; // 停止位

            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            // 注册数据接收事件
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.ErrorReceived += SerialPort_Error;

            try
            {
                _serialPort.Open();
                Tips_Show($"已打开 {portName}，等待数据...");

            }
            catch (Exception ex)
            {
                Tips_Show($"打开串口发生错误: {ex.Message}");
            }

        }

        private void PLCInit(string com)
        {
            // 配置串口参数
            string portName = com;       // 端口号
            int baudRate = 9600;            // 波特率
            Parity parity = Parity.None;   // 校验位
            int dataBits = 8;              // 数据位
            StopBits stopBits = StopBits.One; // 停止位

            PLC = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            try
            {
                PLC.Open();
                Tips_Show($"已打开 {portName}，等待数据...");

            }
            catch (Exception ex)
            {
                Tips_Show($"打开串口发生错误: {ex.Message}");
            }

        }

        private void SerialPort_Error(object sender, SerialErrorReceivedEventArgs e)
        {
            _serialPort.Close();
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //SendNG_End();
                string data = _serialPort.ReadTo("\r");
                Tips_Show($"串口读取收到: {data}");
                List<string> Datas = data.Split(';').ToList();
                string index = string.Empty;
                Regex regex = new Regex(InputRegex.Text);

                Invoke(new Action(() =>
                {
                    for (int i = 0; i < Datas.Count; i++)
                    {
                        //输入1 - 8填入
                        switch (i)
                        {
                            case 0:
                                Input1.Text = Datas[0];
                                break;
                            case 1:
                                Input2.Text = Datas[1];
                                break;
                            case 2:
                                Input3.Text = Datas[2];
                                break;
                            case 3:
                                Input4.Text = Datas[3];
                                break;
                            case 4:
                                Input5.Text = Datas[4];
                                break;
                            case 5:
                                Input6.Text = Datas[5];
                                break;
                            case 6:
                                Input7.Text = Datas[6];
                                break;
                            case 7:
                                Input8.Text = Datas[7];
                                break;

                        }
                    }
                }));
                

                Datas.RemoveAll(x => x == "NG");

                //扫码数量少于拼板数量
                if (!AllBind.Checked)
                {
                    if (Datas.Count() < int.Parse(Num.Text))
                    {
                        //DialogResult result = MessageBox.Show($"仅获取{Datas.Count}个数据，是否按打叉板绑定！", "提示", MessageBoxButtons.YesNo);
                        lock (_lock)
                        {
                            IsNG = true;
                            SendNG();
                            MessageBoxView Mess = new MessageBoxView();
                            Mess.ShowTips($"仅获取{Datas.Count}个数据，是否按打叉板绑定！");
                            DialogResult result = Mess.ShowDialog();
                            if (result == DialogResult.Yes)
                            {
                                //SendNG_End();
                            }
                            else
                            {
                                //SendNG_End();
                                InputClear();
                                return;
                            }
                        }

                    }
                }

                for (int i = 0; i < Datas.Count; i++)
                {
                    if (!regex.IsMatch(Datas[i]))
                    {
                        IsNG = true;
                        SendNG();
                        Tips_Show($"{data}存在条码信息与字符格式不符项！取消绑定");
                        return;
                    }
                }
                Bind(Datas);
                InputClear();
            }
            catch (Exception ex)
            {
                Tips_Show($"串口读取错误: {ex.Message}", true);
                IsNG = true;
                SendNG();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Run_SetUp();
            Num.Text = Properties.Settings.Default.Num;
            InputRegex.Text = Properties.Settings.Default.InputRegex;
            InputClear();
            ScanList.DataSource = SerialPort.GetPortNames().ToList();
            ScanList.Text = Properties.Settings.Default.ScanCOM;
            PLCList.DataSource = SerialPort.GetPortNames().ToList();
            PLCList.Text = Properties.Settings.Default.PLCCOM;
            AllBind.Checked = Properties.Settings.Default.AllPack;

        }

        private void Run_SetUp()
        {
            if (IsRun)
            {
                Num.Enabled = false;
                InputRegex.Enabled = false;
                ScanList.Enabled = false;
                PLCList.Enabled = false;
                SaveCOM();
                if (!string.IsNullOrEmpty(ScanList.Text))
                {
                    SerialInit(ScanList.Text);
                }
                if (!string.IsNullOrEmpty(PLCList.Text))
                {
                    PLCInit(PLCList.Text);
                }
            }
            else
            {
                Num.Enabled = true;
                InputRegex.Enabled = true;
                ScanList.Enabled = true;
                PLCList.Enabled = true;
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                }
                if (PLC != null)
                {
                    if (PLC.IsOpen)
                    {
                        PLC.Close();
                    }
                }
                Tips_Show("");
            }
        }

        private void Text_Changed(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            switch (textBox.Name)
            {
                case "Num":
                    if (!string.IsNullOrWhiteSpace(Num.Text))
                    {
                        int num = 0;
                        if (int.TryParse(Num.Text, out num))
                        {
                            if (num > 0)
                            {
                                Properties.Settings.Default.Num = textBox.Text;
                                WriteLogs("拼板数量设置为：" + textBox.Text);
                            }
                            else
                            {
                                Tips_Show("请输入大于0的纯数字", true);
                                Num.Clear();
                            }

                        }
                        else
                        {
                            Tips_Show("请输入大于0的纯数字", true);
                            Num.Clear();
                        }
                    }
                    break;
                case "InputRegex":
                    Properties.Settings.Default.InputRegex = textBox.Text;
                    WriteLogs("字符格式设置为：" + textBox.Text);
                    break;

            }
            Properties.Settings.Default.Save();
        }

        private void SaveCOM()
        {

            Properties.Settings.Default.ScanCOM = ScanList.Text;

            Properties.Settings.Default.PLCCOM = PLCList.Text;

            Properties.Settings.Default.Save();
        }

        private void SetFocusToControl(string controlName)
        {
            Control[] controls = this.Controls.Find(controlName, true);
            if (controls.Length > 0)
            {
                controls[0].Focus();
            }
        }

        private void InputClear()
        {
            for (int i = 1; i <= 8; i++)
            {
                Control[] controls = this.Controls.Find("Input" + i, true);
                if (controls.Length > 0)
                {
                    TextBox textBox = controls[0] as TextBox;
                    textBox.Clear();
                }
            }
            //Input1.Focus();
            //snList = new List<string>();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            Tips.Text = "";
            //if (e.KeyCode == Keys.Enter)
            //{
            //    TextBox textBox = sender as TextBox;

            //    if (Check_Regex(textBox))
            //    {
            //        if (!snList.Contains(textBox.Text))
            //        {
            //            snList.Add(textBox.Text);
            //            WriteLogs("snList新增：" + textBox.Text);
            //            if (snList.Count == int.Parse(Properties.Settings.Default.Num))
            //            {
            //                //string json = ListToJson();
            //                //WriteLogs(json);
            //                //string result = PostUrl(BindUrl, json);
            //                WriteLogs(result);
            //                JObject jsondata = (JObject)JsonConvert.DeserializeObject(result);
            //                if (jsondata["code"].ToString() != "0")
            //                {
            //                    Tips_Show("绑定失败，原因：" + jsondata["message"].ToString(), true);
            //                    snList.Clear();
            //                    InputClear();
            //                }
            //                else
            //                {
            //                    Tips_Show("绑定成功");
            //                    snList.Clear();
            //                    InputClear();

            //                }
            //            }
            //            else
            //            {
            //                SetFocusToControl("Input" + (snList.Count + 1));
            //            }
            //        }
            //        else
            //        {
            //            Tips_Show($"输入值{textBox.Text}已存在，请重新输入", true);
            //            textBox.Clear();
            //        }
            //    }
            //    else
            //    {
            //        Tips_Show($"输入值{textBox.Text}格式错误，请重新输入", true);
            //        textBox.Clear();
            //    }
            //}
        }

        private bool Check_Regex(TextBox textBox)
        {
            bool result = false;
            result = Regex.IsMatch(textBox.Text, Properties.Settings.Default.InputRegex);
            return result;
        }

        private void Tips_Show(string info, bool error = false)
        {
            Invoke(new Action(() =>
            {
                if (!string.IsNullOrEmpty(info))
                {
                    Tips.Text = DateTime.Now.ToString("HH:mm:ss") + "--" + info;
                }
                else
                {
                    Tips.Text = "";
                }

                if (error)
                {
                    Tips.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    Tips.ForeColor = System.Drawing.Color.Green;
                }
                WriteLogs(info);
            }));

        }

        private void WriteLogs(string Info)
        {
            string logInfo = DateTime.Now.ToString("HH:mm:ss--") + Info;
            string FloderPath = AppDomain.CurrentDomain.BaseDirectory;
            string FilePath = FloderPath.Replace("/", "\\") + "Logs\\";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            FilePath += DateTime.Now.ToString("yyyyMMdd") + "log.txt";
            StreamWriter writer = new StreamWriter(FilePath, true, System.Text.Encoding.UTF8);
            writer.WriteLine(logInfo);
            writer.Close();
        }

        private string ListToJson(List<string> snList)
        {
            var obj = new { snList };
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }

        public string PostUrl(string Url, string jsonStr = "")
        {
            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/json";
            //设置参数，并进行URL编码 
            string paraUrlCoded = jsonStr;//System.Web.HttpUtility.UrlEncode(jsonParas);   
            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 
            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                writer = null;
                throw ex;
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流
                           // String strValue = "";//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();
            //  Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            return postContent;//返回Json数据
        }

        private void Bind(List<string> list)
        {
            string json = ListToJson(list);
            WriteLogs(json);
            string result = PostUrl(BindUrl, json);
            WriteLogs(result);
            JObject jsondata = (JObject)JsonConvert.DeserializeObject(result);
            if (jsondata["code"].ToString() != "0")
            {
                Tips_Show($"{json}绑定失败，原因：" + jsondata["message"].ToString(), true);
                InputClear();
                IsNG = true;
                SendNG();
            }
            else
            {
                Tips_Show($"{json}绑定成功");
                Invoke(new Action(() =>
                {
                    BindCount += list.Count;
                    BindNum.Text = BindCount.ToString();
                }));
                
                InputClear();
                SendPASS();
            }
        }

        private void ClearBindNum_Btn_Click(object sender, EventArgs e)
        {
            BindCount = 0;
            BindNum.Text = BindCount.ToString();
        }

        private void Run_Btn_Click(object sender, EventArgs e)
        {
            if (ScanList.Items.Count == 0)
            {
                Tips_Show("串口选项为空，检查串口线连接！");
                return;
            }
            if (string.IsNullOrEmpty(Num.Text))
            {
                Tips_Show("拼板数量为空，检查实拼板数量！");
                return;
            }
            if (!IsRun)
            {
                IsRun = true;
                Invoke(new Action(() =>
                {
                    Run_Btn.Text = "运行中";
                    Run_Btn.BackColor = Color.Green;
                }));
            }
            else
            {
                IsRun = false;
                Invoke(new Action(() =>
                {
                    Run_Btn.Text = "未运行";
                    Run_Btn.BackColor = Color.Red;
                }));
            }
            Run_SetUp();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
        }

        private void AllPack_CheckStateChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AllPack = AllBind.Checked;
            Properties.Settings.Default.Save();
        }

        private async void PLCSend(string cmd)
        {
            await PLCLock.WaitAsync();
            try
            {
                byte[] bytes = HexToBytes(cmd);
                PLC.Write(bytes, 0, bytes.Length);
                await Task.Delay(50);
            }
            finally
            {
                PLCLock.Release();
            }
        }

        private async void SendNG()
        {


            await Task.Run(async () =>{
                //while (IsNG)
                //{
                    PLCSend(NGCmd);
                    await Task.Delay(1000);
                //    PLCSend(NGEnd);
                //}
            });
            
                //PLCSend(NGCmd);
            //    await Task.Run(() => { Thread.Sleep(1000); });
            //    PLCSend(NGEnd);
            //}


        }

        private async void SendPASS()
        {

            await Task.Run(async () => {
                PLCSend(PASSCmd);
                await Task.Delay(1000);
                PLCSend(PASSCmd);
            });
            
            //await Task.Delay(1000);
            //PLCSend(PASSEnd);


        }

        private void SendPASS_End()
        {
            PLCSend(PASSEnd);
        }

        private void SendNG_End()
        {
            IsNG = false;
            //PLCSend(NGEnd);
            //PLCSend(ResetCmd);
        }

        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }

        private void TipsStop_Click(object sender, EventArgs e)
        {
            SendNG_End();
        }
    }

}
