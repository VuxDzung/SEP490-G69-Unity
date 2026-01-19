namespace SEP490G69
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CurrenciesManager : MonoBehaviour, IGameContext
    {
        private readonly Dictionary<string, int> _currenciesLookUp = new Dictionary<string, int>();

        private EventManager _eventManager;

        public void SetManager(ContextManager manager)
        {
            _eventManager = manager.ResolveGameContext<EventManager>();
        }

        public void AddCurrency(string currency, int amount)
        {
            if (_currenciesLookUp.ContainsKey(currency))
            {
                _currenciesLookUp[currency] += amount;
            }
            else
            {
                _currenciesLookUp.Add(currency, amount);
            }
        }

        public bool TrySpendCurrency(string currency, int amount)
        {
            if (!_currenciesLookUp.ContainsKey(currency)) return false;

            if (_currenciesLookUp[currency] < amount) return false;

            _currenciesLookUp[currency] -= amount;

            return true;
        }
    }
}