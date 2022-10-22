using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoHelper.Controls;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Plugins
{
    internal class MicrostationPlugin
    {
        public MicrostationPlugin(List<TableNodesBase> nodes)
        {
            _nodes = nodes;
        }

        static string _ipAddress = "127.0.0.1";
        static int _port = 14096;
        readonly List<TableNodesBase> _nodes;

        public void Send()
        {
            var client = new TcpClient();
            try
            {
                client.Connect(_ipAddress, _port);
            }
            catch (SocketException)
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                MessageBox.Show(dictionary.Translate<string>("372", "Text"), "Error", MessageBoxButton.OK);
                return;
            }
            finally
            {
                client.Close();
            }
            var dlg = new ProgressBarDialog(TransferIntoMicrostation);
            dlg.ShowDialog();
        }

        public void TransferIntoMicrostation(ProgressBarDialog dialog, CancellationToken ct)
        {
            int counter = 0;
            var client = new TcpClient();
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            try
            {
                client.Connect(serverEndPoint);
                NetworkStream clientStream = client.GetStream();
                foreach (TableNodesBase node in _nodes)
                {
                    if (node is TableCoordinateListNode)
                    {
                        if (ct.IsCancellationRequested)
                            break;
                        /*
                        TableCoordinateListNode ss = (TableCoordinateListNode)TableCoordinateListNode;
                        string info = "0;";
                        info += ss.Prefix.ToString(CultureInfo.InvariantCulture) + ";";
                        info += ss.Number.ToString(CultureInfo.InvariantCulture) + ";";
                        info += ss.X.ToString(CultureInfo.InvariantCulture) + ";";
                        info += ss.Y.ToString(CultureInfo.InvariantCulture);
                        info += "\n";
                        ASCIIEncoding encoder = new ASCIIEncoding();
                        byte[] buffer = encoder.GetBytes(info);
                        clientStream.Write(buffer, 0, buffer.Length);
                        clientStream.Flush();
                         */
                        var ss = (TableCoordinateListNode) node;
                        clientStream.WriteInt32(1);
                        clientStream.WriteString(ss.Prefix);
                        clientStream.WriteString(ss.Number);
                        clientStream.WriteDouble(ss.X);
                        clientStream.WriteDouble(ss.Y);
                        var b = new byte[1];
                        clientStream.Read(b, 0, 1);
                        if (b[0] == 0)
                        {
                            throw new NotImplementedException();
                        }
                        if (counter%5 == 0)
                        {
                            dialog.ProgressBarValue = counter/(double) _nodes.Count*100.0;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    counter++;
                }
            }
            catch (Exception)
            {
                var par = new ResourceParams();
                par.Add("bod", _nodes[counter].NumberWithPrefix);
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                MessageBox.Show(dictionary.Translate("373", "Text", par), "Error", MessageBoxButton.OK);
            }
            finally
            {
                if (client.Connected)
                    client.Close();
            }
            dialog.ProgressBarValue = 100;
        }
    }
}