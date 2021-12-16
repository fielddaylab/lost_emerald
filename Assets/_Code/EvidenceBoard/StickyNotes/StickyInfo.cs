using System;
using BeauUtil;
using BeauUtil.Blocks;

namespace Shipwreck {

    public class StickyInfo : IDataBlock {

        public enum LocationType {
            Last,
            First,
            Anywhere,
			Only
        }
        
        public enum ResponseType {
			Unassigned = 0,
            Correct,
            Hint,
            Incorrect
        }

        private string m_fullId;
        private StringHash32[] m_roots;
        private StringHash32[] m_nodeIds;
        private StringHash32[] m_predecessors;
        private LocationType m_location;
        private ResponseType m_response;
		private StringHash32[] m_requiredChains;
		private bool m_noDangler;

		[BlockContent] private string m_text;
        private int m_specificity;

        public StickyInfo(string fullId) {
            m_fullId = fullId;
        }

        public StringHash32 Id {
            get { return m_fullId; }
        }

        public string Name {
            get { return m_fullId; }
        }

        public ListSlice<StringHash32> RootIds {
            get { return m_roots; }
        }

        public ListSlice<StringHash32> NodeIds {
            get { return m_nodeIds; }
        }

        public ListSlice<StringHash32> Prerequisites {
            get { return m_predecessors; }
        }

		public ListSlice<StringHash32> RequiredChains {
			get { return m_requiredChains; }
		}

		public LocationType Location {
            get { return m_location; }
        }

        public ResponseType Response {
            get { return m_response; }
        }

        public string Text {
            get { return m_text; }
        }

        public StringHash32 TextId {
            get { return m_fullId; }
        }
        
        public int Specificity {
            get { return m_specificity; }
        }

		public bool NoDangler {
			get { return m_noDangler; } // if a hint, does this not use another pin at the end?
		}

		#region Block Meta

		[BlockMeta("root")]
        private void SetRoots(StringSlice argsList) {
            m_roots = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
        }

        [BlockMeta("correct")]
        private void SetCorrect(StringSlice argsList) {
            m_response = ResponseType.Correct;
            m_nodeIds = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
            m_specificity += m_nodeIds.Length;
        }

        [BlockMeta("hint")]
        private void SetHint(StringSlice argsList) {
            m_response = ResponseType.Hint;
            m_nodeIds = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
            m_specificity += m_nodeIds.Length;
        }

        [BlockMeta("incorrect")]
        private void SetIncorrect(StringSlice argsList) {
            m_response = ResponseType.Incorrect;
            m_nodeIds = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
            m_specificity += m_nodeIds.Length;
        }

        [BlockMeta("precededBy")]
        private void SetPreceded(StringSlice argsList) {
            m_predecessors = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
            m_specificity += m_predecessors.Length;
        }

        [BlockMeta("anywhere")]
        private void SetAnywhere() {
            m_location = LocationType.Anywhere;
        }

        [BlockMeta("last")]
        private void SetLast() {
            m_location = LocationType.Last;
        }

        [BlockMeta("first")]
        private void SetFirst() {
            m_location = LocationType.First;
            m_predecessors = null;
        }
		
		[BlockMeta("only")]
		private void SetOnly() {
			m_location = LocationType.Only;
			m_predecessors = null;
		}

		[BlockMeta("requires")]
		private void SetRequires(StringSlice argsList) { 
			m_requiredChains = ArrayUtils.MapFrom<StringSlice, StringHash32>(GetArgs(argsList), (s) => s);
			m_specificity += m_requiredChains.Length;
		}
		[BlockMeta("noDangler")]
		private void SetNoDangler() {
			m_noDangler = true;
		}

        #endregion // Block Meta

        #region Utils

        static private TempList16<StringSlice> GetArgs(StringSlice argsList) {
            TempList16<StringSlice> args = default(TempList16<StringSlice>);
            argsList.Split(ScriptNode.ArgsSplitter, StringSplitOptions.None, ref args);
            return args;
        }

        #endregion // Utils
    }

}