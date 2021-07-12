using System;
using BeauPools;
using BeauUtil;
using BeauUtil.Variants;

namespace Shipwreck {
    
    public struct TempVarTable : IDisposable {
        private readonly TempAlloc<VariantTable> m_table;

        internal TempVarTable(TempAlloc<VariantTable> tempTable) {
            m_table = tempTable;
        }

        public void Set(StringHash32 id, Variant value) {
            m_table.Object?.Set(id, value);
        }

        void IDisposable.Dispose() {
            m_table.Dispose();
        }

        static public implicit operator VariantTable(TempVarTable table)
        {
            return table.m_table?.Object;
        }

        static public TempVarTable Alloc()
        {
            return new TempVarTable(s_pool.TempAlloc());
        }

        static private readonly IPool<VariantTable> s_pool;

        static TempVarTable()
        {
            s_pool = new DynamicPool<VariantTable>(4, Pool.DefaultConstructor<VariantTable>());
            s_pool.Config.RegisterOnFree((p, t) => t.Clear());
        }
    }
}