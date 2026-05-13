using System.Collections.Generic;
using System.Linq;
using CustomTypes;
using Units;
using UnityEngine;

namespace Infrastructure
{
    public class StaticDataService
    {
        private Dictionary<string, UnitConfig> _unitDatas;

        public void Init()
        {
            _unitDatas = Resources
                .LoadAll<UnitConfig>(Constants.UNIT_DATA_PATH)
                .Where(data => data.IsEnabled)
                .ToDictionary(data => data.Id, data => data);
        }

        public UnitConfig GetUnit(string id)
            => TryGetData(_unitDatas, id);

        public UnitConfig GetAnyUnit(UnitSpecialization specialization)
            => _unitDatas.Values.FirstOrDefault(data => data.Specialization.Equals(specialization));

        private TConfig TryGetData<TId, TConfig>(IReadOnlyDictionary<TId, TConfig> datas, TId id)
            => datas.GetValueOrDefault(id);
    }
}