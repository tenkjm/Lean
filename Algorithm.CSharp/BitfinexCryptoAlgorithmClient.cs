using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Securities;
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Orders;
using Newtonsoft.Json;
using QuantConnect.NetworkUtil;

namespace QuantConnect.Algorithm.CSharp
{

    public class BitfinexCryptoAlgorithmClient: QCAlgorithm
    {
        Security eth;
        TcpClientSsl tcpClient;

        public override void Initialize()
        {
            tcpClient = new TcpClientSsl("195.54.162.74", 10000, "certificate.pfx", "1");

            SetBrokerageModel(BrokerageName.Bitfinex);      // by default this will be AccountType.Margin type account
            eth = AddCrypto("ETHUSD", market: Market.Bitfinex);
        }

        //сюда приходят данные о биржевых котировках, они инкапсулированы в объект Slice
        public override void OnData(Slice slice)
        {
            //var closePrice = slice.Bars[eth.Symbol].Close;
            //Log($"current price: {closePrice}");
            
            tcpClient.SendMessage("I am sending a test message every minute");

            /*
            Symbol ethSymbol = Symbol.Create("ETHUSD", SecurityType.Crypto, null);
            var buySignal = new TradingSignal(ethSymbol, OrderDirection.Buy, DateTime.Now);
            tcpClient.SendMessage(JsonConvert.SerializeObject(buySignal) + "\n\r");
            */
            
            /*
            //Eсли это первая итерация - создадим сигнал на покупку 
            if(counter ==0)
            {
                //Cоздаем объект сигнал. Time - это свойство объекта QCAlgorithm - суть текущее время
                var signal = new TradingSignal(eth.Symbol, OrderDirection.Buy, Time);
                MoneyManagement(signal);
            }

            //увеличиваем счетчик
            counter++;
            */
        }

        public override void OnEndOfAlgorithm()
        {
            tcpClient.Disconnect();
        }
    }
}
