using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace BaiTapLyThuyet5._5
{
    public partial class Form1 : Form
    {
        private IMailFolder inbox;
        public Form1()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (send_emailBox.Text == "")
            {
                MessageBox.Show("Email trống");
                return;
            }
            if (pwdBox.Text == "")
            {
                MessageBox.Show("Password trống");
                return;
            }



            var client = new ImapClient();
            using (var cancel = new CancellationTokenSource())
            {
                client.Connect("find-uit.f1301.cyou", 993,true, cancel.Token);
                client.AuthenticationMechanisms.Remove("XOAUTH");
                client.Authenticate(send_emailBox.Text, pwdBox.Text, cancel.Token);
                inbox = client.Inbox;

                inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var content = inbox.GetMessage(i);
                    ListViewItem name = new ListViewItem(content.Subject);
                    ListViewItem.ListViewSubItem from = new ListViewItem.ListViewSubItem(name, content.From.ToString());
                    name.SubItems.Add(from);
                    inboxBox.Items.Add(name);
                }
            }
        }

        private void inboxBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void inboxBox_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < inboxBox.Items.Count; i++)
            {
                var box = inboxBox.GetItemRect(i);
                if (box.Contains(e.Location))
                {
                    var content = inbox.GetMessage(i);
                    string from, subject, to, body;
                    from = "";
                    subject = "";
                    to = "";
                    body = "";
                    if (content.From != null)
                    {
                        from = content.From.ToString();
                    }
                    if (content.Subject != null)
                    {
                        subject = content.Subject.ToString();
                    }
                    if (content.To != null)
                    {
                        to = content.To.ToString();
                    }
                    if (content.TextBody != null)
                    {
                        body = content.TextBody.ToString();
                    }

                    new Inbox(from, subject, to, body).Show();
                    return;
                }
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            string from = send_emailBox.Text.Trim();
            string to = to_emailBox.Text.Trim();
            string password = pwdBox.Text.Trim();
            string body = contentBox.Text.Trim();
            string subject = subjectBox.Text.Trim();
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(from);
            mail.Subject = subjectBox.Text;
            mail.Body = contentBox.Text;
            if (pathBox.Text != "")
            {
                Attachment attachment = new Attachment(pathBox.Text);
                mail.Attachments.Add(attachment);
            }

            SmtpClient smtpClient = new SmtpClient("find-uit.f1301.cyou");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from, password);

            smtpClient.Send(mail);

           
        }


        private void attachBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pathBox.Text = ofd.FileName;
            }
        }
    }
}
