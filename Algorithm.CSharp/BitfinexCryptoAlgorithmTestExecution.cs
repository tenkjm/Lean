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

namespace QuantConnect.Algorithm.CSharp
{
    //это объект, который реализует сигнал, указание на покупку продажу криптовалютной пары
    public class TradingSignal
    {
        private Symbol m_symbol;
        private OrderDirection m_direction;
        private DateTime m_timestamp;
        //инструмент , сивмол
        public Symbol Symbol
        {
            get { return m_symbol; }
            set { m_symbol = value; }
        }
        //направление - купить, продать
        public OrderDirection Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }
        //время создания сигнала
        public DateTime Time
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

        public TradingSignal(Symbol symbol, OrderDirection direction, DateTime timestamp)
        {
            m_symbol = symbol;
            m_direction = direction;
            m_timestamp = timestamp;
        }
    }

    //весь наш тестовый алгоритм заключен в этом классе
    public class BitfinexCryptoAlgorithmTestExecution: QCAlgorithm
    {
        Security eth;

        public override void Initialize()
        {
            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            eth = AddCrypto("BTCUSD", Resolution.Minute, Market.Bitfinex);
        }

        //метод вызываетс при любом изменения статуса или параметра активного ордера
        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            //это ордер, статус которого изменился
            Order order = Transactions.GetOrderById(orderEvent.OrderId);
            //например запишем в консоль значение поля tag ордера
            Console.WriteLine(order.Tag);
        }

        //сюда приходят данные о биржевых котировках, они инкапсулированы в объект Slice
        public override void OnData( Slice slice)
        {
            ////Eсли это первая итерация - создадим сигнал на покупку 
            //if(counter ==0)
            //{
            //    //Cоздаем объект сигнал. Time - это свойство объекта QCAlgorithm - суть текущее время
            //    var signal = new TradingSignal(eth.Symbol, OrderDirection.Buy, Time);
            //    MoneyManagement(signal);
            //}

            ////увеличиваем счетчик
            //counter++;
        }


        public void GetSignal(Object sender, string remoteData)
        {
            //Cоздаем объект сигнал. Time - это свойство объекта QCAlgorithm - суть текущее время
            var signal = JsonConvert.DeserializeObject<TradingSignal>(remoteData);
            MoneyManagement(signal);
            
        }
        

        private void MoneyManagement(TradingSignal signal)
        {
            //размер минимального ордера по данному инструменту можно узнать так
            var minOrderQuantity = 0.06m;
            
            //один из параметров ордера - поле tag - 
            //суть строка, которая может содержать любую полезную для трейдера информацию
            //например мы здесь запишем совокупное количество инстурмента, которое мы хотим исполнить
            //и цену инстурмента на момент исполнения
            string orderTag = String.Format($"100.0,{eth.Price}");

            //отправим ордер на биржу
            if(signal.Direction == OrderDirection.Buy)
            {
                MarketOrder(signal.Symbol, minOrderQuantity, tag: orderTag);
            }
            
        }
        
    }
}
