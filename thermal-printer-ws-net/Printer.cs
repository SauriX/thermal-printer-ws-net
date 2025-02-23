using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using thermalPrinterWsNet.Models;
namespace thermalPrinterWsNet
{
    public class Printer
    {
        private ClientWebSocket _ws;
        private string _printerName;
        private Uri _uri;
        private List<PrintCommand> _printList;

        public Printer(string printerName = null, string ip = "localhost")
        {
            _printerName = printerName;
            _uri = new Uri($"ws://{ip}:9090");
            _ws = new ClientWebSocket();
            _printList = new List<PrintCommand>();


        }
        // Método asincrónico para conectar
        public async Task InitializeAsync()
        {
            await Connect();
        }
        private async Task Connect()
        {
            try
            {
                await _ws.ConnectAsync(_uri, CancellationToken.None);
                Console.WriteLine($"Conectado a la impresora: {_printerName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar con la impresora: {ex.Message}");
            }
        }

        public void AddCommand(string action, string text = null, int count = 0, bool mode = false, string imagePath = null)
        {
            var command = new PrintCommand
            {
                Action = action,
                Text = text,
                Count = count,
                Mode = mode,
                ImagePath = imagePath
            };
            _printList.Add(command);
        }

        public async Task SendCommands()
        {
            try
            {
                if (_ws.State == WebSocketState.Open)
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { PrinterName = _printerName, Commands = _printList });
                    var buffer = Encoding.UTF8.GetBytes(json);
                    await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"Comandos enviados: {json}");
                }
                else
                {
                    Console.WriteLine("Esperando conexión WebSocket...");
                    Connect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar comandos: {ex.Message}");
            }
        }

        public async Task<List<string>> GetPrinters()
        {
            var printers = new List<string>();
            try
            {
                if (_ws.State == WebSocketState.Open)
                {
                    var request = "printers";
                    var buffer = Encoding.UTF8.GetBytes(request);
                    await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine("Comando enviado: printers");

                    // Escuchar la respuesta
                    var responseBuffer = new byte[1024];
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
                    var response = Encoding.UTF8.GetString(responseBuffer, 0, result.Count);
                    var printersjson = Newtonsoft.Json.JsonConvert.DeserializeObject<PrintList>(response);
                    printers = printersjson.printers;
                    Console.WriteLine($"Respuesta recibida: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener impresoras: {ex.Message}");
            }
            return printers;
        }

        // Funciones de impresión
        public void PrintText(string text) => AddCommand("text", text);
        public void CutPartial() => AddCommand("partial");
        public void CutFull() => AddCommand("full");
        public void PrintDocument()
        {
            AddCommand("printDocument");
            SendCommands().Wait();
        }
        public void TestPrinter() => AddCommand("testPrinter");
        public void Code123(string text) => AddCommand("code123", text);
        public void Code39(string text) => AddCommand("code39", text);
        public void Ean13(string text) => AddCommand("ean13", text);
        public void OpenDrawer() => AddCommand("openDrawer");
        public void Separator(string text) => AddCommand("separator", text);
        public void Bold(string text) => AddCommand("bold", text);
        public void Underline(string text) => AddCommand("underLine", text);
        public void Expanded(bool mode) => AddCommand("expanded", null, 0, mode);
        public void Condensed(bool mode) => AddCommand("condensed", null, 0, mode);
        public void DoubleWidth2() => AddCommand("doubleWidth2");
        public void DoubleWidth3() => AddCommand("doubleWidth3");
        public void NormalWidth() => AddCommand("normalWidth");
        public void AlignRight() => AddCommand("right");
        public void AlignCenter() => AddCommand("center");
        public void AlignLeft() => AddCommand("left");
        public void FontA(string text) => AddCommand("fontA", text);
        public void FontB(string text) => AddCommand("fontB", text);
        public void FontC(string text) => AddCommand("fontC", text);
        public void FontD(string text) => AddCommand("fontD", text);
        public void FontE(string text) => AddCommand("fontE", text);
        public void FontSpecialA(string text) => AddCommand("fontSpecialA", text);
        public void FontSpecialB(string text) => AddCommand("fontSpecialB", text);
        public void InitializePrint() => AddCommand("initializePrint");
        public void LineHeight(int count) => AddCommand("lineHeight", null, count);
        public void NewLines(int count) => AddCommand("newLines", null, count);
        public void NewLine() => AddCommand("newLine");
    }

  


}