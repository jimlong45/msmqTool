namespace MSMQ
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Messaging;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    public class Form1 : Form
    {
        private Button btnR;
        private Button btnSend;
        private Button button1;
        private CheckBox chbChoose;
        private CheckBox chbChoose1;
        private CheckBox checkBox2;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox txtMSMQ;
        private TextBox txtMSMQ1;
        private TextBox txtPath;
        private Label label7;
        private ComboBox comboBox1;
        private TextBox txtPath1;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void btnR_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chbChoose1.Checked)
                {
                    MessageQueue queue = new MessageQueue(this.txtMSMQ1.Text) {
                        Formatter = new BinaryMessageFormatter()
                    };
                    System.Messaging.Message message = new System.Messaging.Message();
                    MessageQueueTransaction transaction = new MessageQueueTransaction();
                    transaction.Begin();
                    try
                    {
                        message = queue.Receive(TimeSpan.FromMilliseconds(500.0), transaction);
                    }
                    catch
                    {
                        transaction.Commit();
                    }
                    StreamReader reader = new StreamReader(message.BodyStream, Encoding.ASCII);
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.txtPath1.Text = this.saveFileDialog1.FileName;
                        FileStream output = new FileStream(this.txtPath1.Text, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        BinaryWriter writer = new BinaryWriter(output);
                        writer.Write(Encoding.Default.GetBytes(reader.ReadToEnd()));
                        writer.Flush();
                        writer.Close();
                        transaction.Commit();
                        MessageBox.Show("发送成功！");
                    }
                }
                else
                {
                    MessageQueue queue2 = new MessageQueue(this.txtMSMQ1.Text) {
                        Formatter = new BinaryMessageFormatter()
                    };
                    System.Messaging.Message message2 = new System.Messaging.Message();
                    StreamReader reader2 = new StreamReader(queue2.Receive(TimeSpan.FromMilliseconds(500.0)).BodyStream, Encoding.ASCII);
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.txtPath1.Text = this.saveFileDialog1.FileName;
                        FileStream stream2 = new FileStream(this.txtPath1.Text, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        BinaryWriter writer2 = new BinaryWriter(stream2);
                        writer2.Write(Encoding.Default.GetBytes(reader2.ReadToEnd()));
                        writer2.Flush();
                        writer2.Close();
                        MessageBox.Show("发送成功！");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("发送失败！原因：" + exception.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chbChoose.Checked)
                {
                    MessageQueue queue = new MessageQueue(this.txtMSMQ.Text);
                    System.Messaging.Message message = new System.Messaging.Message {
                        Label = Guid.NewGuid().ToString()
                    };
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.txtPath.Text = this.openFileDialog1.FileName;
                        FileStream stream = new FileStream(this.txtPath.Text, FileMode.Open, FileAccess.Read);
                        int length = (int) stream.Length;
                        byte[] buffer = new byte[length];
                        stream.Read(buffer, 0, length);
                        MemoryStream stream2 = new MemoryStream(buffer);
                       
                        if (comboBox1.Text.Equals("海关"))
                        {
                            message.BodyStream = stream2;
                            queue.Send(message, MessageQueueTransactionType.Single);
                            stream.Close();                      
                        }
                        else if (comboBox1.Text.Equals("园区"))
                        {
                            string msgStr = System.Text.Encoding.UTF8.GetString(stream2.ToArray());
                            string key = "pwd";
                            msgStr = AESUtil.AesEncoding(msgStr, key, Encoding.UTF8);
                            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                            message.Body = msgStr;
                            queue.Send(message, MessageQueueTransactionType.Single);
                            stream.Close();
                        }
                        else
                        {
                            MessageBox.Show("请选择发送地点！ 海关/园区");
                            stream.Close();
                            return;
                        }
                        
                        MessageBox.Show("发送成功！");
                    }
                }
                else
                {
                    MessageQueue queue2 = new MessageQueue(this.txtMSMQ.Text);
                    System.Messaging.Message message3 = new System.Messaging.Message {
                        Label = Guid.NewGuid().ToString()
                    };
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.txtPath.Text = this.openFileDialog1.FileName;
                        FileStream stream3 = new FileStream(this.txtPath.Text, FileMode.Open, FileAccess.Read);
                        int count = (int) stream3.Length;
                        byte[] buffer2 = new byte[count];
                        stream3.Read(buffer2, 0, count);
                        MemoryStream stream4 = new MemoryStream(buffer2);
                        message3.BodyStream = stream4;
                        queue2.Send(message3);
                        stream3.Close();
                        MessageBox.Show("发送成功！");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("发送失败！原因：" + exception.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.txtMSMQ = new System.Windows.Forms.TextBox();
            this.chbChoose = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnR = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPath1 = new System.Windows.Forms.TextBox();
            this.chbChoose1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMSMQ1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "报文路径：";
            // 
            // txtMSMQ
            // 
            this.txtMSMQ.Location = new System.Drawing.Point(78, 13);
            this.txtMSMQ.Name = "txtMSMQ";
            this.txtMSMQ.Size = new System.Drawing.Size(374, 21);
            this.txtMSMQ.TabIndex = 2;
            this.txtMSMQ.Text = "FormatName:Direct=TCP:192.168.67.2\\private$\\LITB_DECL_RESP_APL";
            this.txtMSMQ.TextChanged += new System.EventHandler(this.txtMSMQ_TextChanged);
            // 
            // chbChoose
            // 
            this.chbChoose.AutoSize = true;
            this.chbChoose.Location = new System.Drawing.Point(554, 15);
            this.chbChoose.Name = "chbChoose";
            this.chbChoose.Size = new System.Drawing.Size(72, 16);
            this.chbChoose.TabIndex = 3;
            this.chbChoose.Text = "是否事务";
            this.chbChoose.UseVisualStyleBackColor = true;
            this.chbChoose.CheckedChanged += new System.EventHandler(this.chbChoose_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(78, 37);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(551, 21);
            this.txtPath.TabIndex = 4;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(549, 64);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(640, 156);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.chbChoose);
            this.tabPage1.Controls.Add(this.btnSend);
            this.tabPage1.Controls.Add(this.txtPath);
            this.tabPage1.Controls.Add(this.txtMSMQ);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(632, 130);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "发送";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "海关",
            "园区"});
            this.comboBox1.Location = new System.Drawing.Point(78, 86);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "地址：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnR);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtPath1);
            this.tabPage2.Controls.Add(this.chbChoose1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtMSMQ1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(632, 130);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "接收";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnR
            // 
            this.btnR.Location = new System.Drawing.Point(549, 57);
            this.btnR.Name = "btnR";
            this.btnR.Size = new System.Drawing.Size(75, 23);
            this.btnR.TabIndex = 9;
            this.btnR.Text = "接收";
            this.btnR.UseVisualStyleBackColor = true;
            this.btnR.Click += new System.EventHandler(this.btnR_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "报文路径：";
            // 
            // txtPath1
            // 
            this.txtPath1.Location = new System.Drawing.Point(75, 30);
            this.txtPath1.Name = "txtPath1";
            this.txtPath1.Size = new System.Drawing.Size(551, 21);
            this.txtPath1.TabIndex = 8;
            // 
            // chbChoose1
            // 
            this.chbChoose1.AutoSize = true;
            this.chbChoose1.Location = new System.Drawing.Point(554, 8);
            this.chbChoose1.Name = "chbChoose1";
            this.chbChoose1.Size = new System.Drawing.Size(72, 16);
            this.chbChoose1.TabIndex = 6;
            this.chbChoose1.Text = "是否事务";
            this.chbChoose1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "地址：";
            // 
            // txtMSMQ1
            // 
            this.txtMSMQ1.Location = new System.Drawing.Point(55, 6);
            this.txtMSMQ1.Name = "txtMSMQ1";
            this.txtMSMQ1.Size = new System.Drawing.Size(485, 21);
            this.txtMSMQ1.TabIndex = 5;
            this.txtMSMQ1.Text = "saintgao\\private$\\test";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(549, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "报文路径：";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(554, 15);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(72, 16);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "是否事务";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "地址：";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(78, 37);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(551, 21);
            this.textBox2.TabIndex = 4;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(55, 13);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(485, 21);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = "saintgao\\private$\\test";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(640, 156);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "MSMQ测试工具";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        private void chbChoose_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtMSMQ_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

  

   



  
    }
}

