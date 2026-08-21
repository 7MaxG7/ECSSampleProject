using System.Collections.Generic;
using System.Linq;
using CustomTypes;
using Units;
using UnityEngine;

namespace Infrastructure
{
    public class StaticDataService
    {
        private Dictionary<string, UnitConfig> _unitConfigs;

        public void Init()
        {
            _unitConfigs = Resources
                .LoadAll<UnitConfig>(Constants.UNIT_DATA_PATH)
                .Where(data => data.IsEnabled)
                .ToDictionary(data => data.Id, data => data);
        }

        public UnitConfig GetUnit(string id)
            => GetData(_unitConfigs, id);

        public UnitConfig GetAnyUnit(UnitSpecialization specialization)
            => _unitConfigs.Values.FirstOrDefault(data => data.Specialization.Equals(specialization));

        private TConfig GetData<TId, TConfig>(IReadOnlyDictionary<TId, TConfig> datas, TId id)
            => datas.GetValueOrDefault(id);
    }
}