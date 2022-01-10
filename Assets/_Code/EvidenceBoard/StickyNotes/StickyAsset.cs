using System;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Blocks;
using BeauUtil.Tags;

namespace Shipwreck {

    public class StickyAsset : ScriptableDataBlockPackage<StickyInfo> {

		[NonSerialized]
        private HashSet<StickyInfo> m_postIts = new HashSet<StickyInfo>();

		public override int Count {
            get { return m_postIts.Count; }
        }

		public override IEnumerator<StickyInfo> GetEnumerator() {
			return m_postIts.GetEnumerator();
		}

        public override void Clear() {
            m_postIts.Clear();
            base.Clear();
        }

        public void Parse() {
            Parse(Generator.Instance);
        }

		private class Generator : GeneratorBase<StickyAsset> {
            static public readonly Generator Instance = new Generator();

			public override bool TryCreateBlock(IBlockParserUtil inUtil, StickyAsset inPackage, TagData inId, out StickyInfo outBlock) {
				string fullName = inUtil.TempBuilder.Append(inUtil.Position.FileName)
                    .Append('-')
                    .Append(inId.Id)
                    .Flush();

                outBlock = new StickyInfo(fullName);
                inPackage.m_postIts.Add(outBlock);
                return true;
			}
		}

        #if UNITY_EDITOR

		[ScriptedExtension(1, "postit")]
        private class Importer : ImporterBase<StickyAsset> {
        }

        static public IEnumerable<KeyValuePair<StringHash32, string>> GetLocalizableContent(StickyAsset asset) {
            asset.Parse();

            foreach(var info in asset) {
                yield return new KeyValuePair<StringHash32, string>(info.TextId, info.Text);
            }
        }

        #endif // UNITY_EDITOR
	}

}