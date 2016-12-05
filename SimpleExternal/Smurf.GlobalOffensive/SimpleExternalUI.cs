using Smurf.GlobalOffensive.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smurf.GlobalOffensive
{
    public partial class SimpleExternalUI : Form
    {
        #region Fields
        private static bool run;
        private static Form UI;
        #endregion

        #region UI Settings

        #region ESP

        #region Glow ESP
        internal static bool glowesp = true;
        internal static bool glowenemy = true;
        internal static bool glowally = true;
        #endregion

        #region Sound ESP
        internal static bool soundesp = true;
        internal static int soundrange = 10;
        internal static int soundvolume = 100;
        internal static int soundinterval = 1000;
        #endregion

        #endregion

        #region Aim bot
        internal static bool aim = true;
        internal static bool aimhumanized = true;
        internal static bool aimspotted = true;
        internal static bool aimenemies = true;
        internal static bool aimallies;
        internal static WinAPI.VirtualKeyShort aimkey;
        internal static int aimfov = 1000; //50
        internal static int aimspeed = 1000; // 50
        internal static int aimbone = 5;
        #endregion

        #region Trigger bot
        internal static bool trigger = true;
        internal static WinAPI.VirtualKeyShort triggerkey = (WinAPI.VirtualKeyShort)Convert.ToInt32("0x12", 16);
        internal static bool triggerenemies = true;
        internal static bool triggerallies = true;
        internal static bool triggerspawnprotection;
        internal static int triggerfirstshotmax = 128;
        internal static int triggerfirstshotmin = 98;
        internal static int triggershotsmax = 68;
        internal static int triggershotsmin = 35;
        internal static bool triggerdash = true;
        internal static bool triggerzoomed = true;
        internal static bool triggerincross;
        internal static bool triggerbone;
        internal static bool triggerhitbox;
        #endregion

        #region Misc

        #endregion

        #endregion

        #region "Methods"
        public SimpleExternalUI()
        {
            InitializeComponent();
            UI = this;
            UI.Show();
            run = true;
            Thread mainThread = new Thread(ThreadStarter);
            mainThread.Start();
        }
        private static void ThreadStarter()
        {
            Thread thread1 = new Thread(PrintInfo);
            Thread thread2 = new Thread(UpdateBhop);
            Thread thread3 = new Thread(UpdateRcs);
            Thread thread4 = new Thread(UpdateSettings);
            Thread thread5 = new Thread(UpdateKeyUtils);
            Thread thread6 = new Thread(UpdateAutoPistol);
            Thread thread7 = new Thread(UpdateAimAssist);
            Thread thread8 = new Thread(UpdateSkinChanger);

            if (UI.Controls[1].InvokeRequired)
                UI.Controls[1].BeginInvoke((MethodInvoker)delegate () { UI.Controls[1].Text = $"Waiting for {Core.GameTitle} to start up.."; ; });
            else UI.Controls[1].Text = $"Waiting for {Core.GameTitle} to start up.."; ;

            while (run && (Core.HWnd = WinAPI.FindWindowByCaption(Core.HWnd, Core.GameTitle)) == IntPtr.Zero)
                Thread.Sleep(250);

            if (run)
            {
                Process[] process = Process.GetProcessesByName("csgo");
                Core.Attach(process[0]);
                StartThreads(thread1, thread2, thread3, thread4, thread5, thread6, thread7, thread8);
            }

            while (run)
            {
                Core.Objects.Update();
                Core.TriggerBot.Update();
                Core.SoundEsp.Update();
                Core.Radar.Update();
                Core.Glow.Update();
                Core.AimAssist.Update();
                Thread.Sleep(1);
            }

            StopThreads(thread1, thread2, thread3, thread4, thread5, thread6, thread7, thread8);
        }
        private static void StopThreads(params Thread[] threads)
        {
            foreach (var thread in threads)
            {
                thread.Abort();
            }
        }
        private static void UpdateSkinChanger()
        {
            while (run)
            {
                Core.SkinChanger.Update();
                Thread.Sleep(1);
            }
        }

        private static void UpdateAimAssist()
        {
            while (run)
            {
                //Core.AimAssist.Update();
                Thread.Sleep(1);
            }
        }

        private static void StartThreads(params Thread[] threads)
        {
            foreach (var thread in threads)
            {
                thread.Priority = ThreadPriority.Highest;
                thread.IsBackground = true;
                thread.Start();
            }
        }
        private static void PrintInfo()
        {
#if DEBUG
            while (run)
            {
                if (UI.Controls[1].InvokeRequired)
                    UI.Controls[1].BeginInvoke((MethodInvoker)delegate () { UI.Controls[1].Text = $"State: {Core.Client.State}"; ; });
                else UI.Controls[1].Text = $"State: {Core.Client.State}"; ;

                if (Core.Client.InGame && Core.LocalPlayer != null && Core.LocalPlayerWeapon != null && Core.LocalPlayer.IsValid && Core.LocalPlayer.IsAlive)
                {
                    var me = Core.LocalPlayer;
                    var myWeapon = Core.LocalPlayerWeapon;
                    /*
                    UI.label1.Text = $"Players: \t{Core.Objects.Players.Count}";
                    UI.label1.Text = $"ID:\t\t{me.Id}";
                    UI.label1.Text = $"Health:\t\t{me.Health}";
                    UI.label1.Text = $"Armor:\t\t{me.Armor}";
                    UI.label1.Text = $"Position:\t{me.Position}";
                    UI.label1.Text = $"Team:\t\t{me.Team}";
                    UI.label1.Text = $"Player Count:\t{Core.Objects.Players.Count}";
                    UI.label1.Text = $"Velocity: \t{me.Velocity}";
                    UI.label1.Text = $"Shots Fired: \t{me.ShotsFired}";
                    UI.label1.Text = $"VecPunch: \t{me.VecPunch}";
                    UI.label1.Text = $"Immune: \t{me.GunGameImmune}";
                    UI.label1.Text = $"Active Weapon: \t{myWeapon.WeaponName}";
                    UI.label1.Text = $"Active Weapon ID: \t{myWeapon.ItemDefinitionIndex}";
                    UI.label1.Text = $"Clip1: \t\t{myWeapon.Clip1}";
                    UI.label1.Text = $"Flags: \t\t{me.Flags}";
                    UI.label1.Text = $"Flash: \t\t{me.FlashMaxAlpha}";
                    UI.label1.Text = $"Weapon Group: \t{myWeapon.WeaponType}";
                    UI.label1.Text = $"Zoom Level: \t{myWeapon.ZoomLevel}";
                    UI.label1.Text = $"Recoil Control Yaw: \t{Core.ControlRecoil.RandomYaw}";
                    UI.label1.Text = $"Recoil Control Pitch: \t{Core.ControlRecoil.RandomPitch}";
                    UI.label1.Text = $"Trigger Delay First: \t{Core.TriggerBot.TriggerDelayFirstRandomize}";
                    UI.label1.Text = $"Trigger Delay Shots1: \t{Core.TriggerBot.TriggerDelayShotsRandomize}";
                    */
                }

                Thread.Sleep(500);
            }
#endif

        }
        private static void UpdateBhop()
        {
            while (run)
            {
                Core.BunnyJump.Update();
                Thread.Sleep(5);
            }
        }
        private static void UpdateRcs()
        {
            while (run)
            {
                Core.ControlRecoil.Update();
                Thread.Sleep(1);
            }
        }
        private static void UpdateSettings()
        {
            while (run)
            {
                Core.Settings.Update();
                Thread.Sleep(10);
            }
        }
        private static void UpdateKeyUtils()
        {
            while (run)
            {
                Core.KeyUtils.Update();
                Core.NoFlash.Update();
                Thread.Sleep(10);
            }
        }
        private static void UpdateAutoPistol()
        {
            while (run)
            {
                Core.AutoPistol.Update();
                Thread.Sleep(1);
            }
        }
        #endregion

        #region UI Events
        private void SimpleExternalUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            run = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            glowesp = checkBox1.Checked;
            checkBox2.Enabled = checkBox1.Checked;
            checkBox3.Enabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            glowenemy = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            glowally = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            soundesp = checkBox4.Checked;
            numericUpDown1.Enabled = checkBox4.Checked;
            numericUpDown2.Enabled = checkBox4.Checked;
            numericUpDown3.Enabled = checkBox4.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            soundrange = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            soundinterval = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            soundvolume = (int)numericUpDown3.Value;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            aim = checkBox5.Checked;
            checkBox6.Enabled = checkBox5.Checked;
            checkBox7.Enabled = checkBox5.Checked;
            checkBox8.Enabled = checkBox5.Checked;
            checkBox9.Enabled = checkBox5.Checked;
            numericUpDown4.Enabled = checkBox5.Checked;
            numericUpDown5.Enabled = checkBox5.Checked;
            numericUpDown6.Enabled = checkBox5.Checked;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            aimfov = (int)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            aimspeed = (int)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            aimbone = (int)numericUpDown6.Value;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            aimhumanized = checkBox6.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            aimspotted = checkBox7.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            aimenemies = checkBox8.Checked;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            aimallies = checkBox9.Checked;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox1.Text = "0x12"; //e.KeyChar.ToString(); (will need to fix the next line first)
            aimkey = (WinAPI.VirtualKeyShort)Convert.ToInt32("0x12", 16); // figure out a way to translate the key char to a virtual key. (needs the proper hex code of the key i.e. 0x{00})
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            trigger = checkBox10.Checked;
            checkBox11.Enabled = checkBox10.Checked;
            checkBox12.Enabled = checkBox10.Checked;
            checkBox13.Enabled = checkBox10.Checked;
            checkBox14.Enabled = checkBox10.Checked;
            checkBox15.Enabled = checkBox10.Checked;
            checkBox16.Enabled = checkBox10.Checked;
            checkBox17.Enabled = checkBox10.Checked;
            checkBox18.Enabled = checkBox10.Checked;
            numericUpDown7.Enabled = checkBox10.Checked;
            numericUpDown8.Enabled = checkBox10.Checked;
            numericUpDown9.Enabled = checkBox10.Checked;
            numericUpDown10.Enabled = checkBox10.Checked;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            triggerenemies = checkBox11.Checked;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            triggerallies = checkBox12.Checked;
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            triggerspawnprotection = checkBox13.Checked;
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            triggerdash = checkBox14.Checked;
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            triggerzoomed = checkBox15.Checked;
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            triggerincross = checkBox16.Checked;
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            triggerbone = checkBox17.Checked;
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            triggerhitbox = checkBox18.Checked;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            triggerfirstshotmin = (int)numericUpDown7.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            triggerfirstshotmax = (int)numericUpDown8.Value;
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            triggershotsmin = (int)numericUpDown10.Value;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            triggershotsmax = (int)numericUpDown9.Value;
        }

        private void SimpleExternalUI_Load(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
