using System;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Blocks;
using BeauUtil.Tags;

namespace Shipwreck {

    public class PostItAsset : ScriptableDataBlockPackage<PostItData> {

		[NonSerialized]
        private HashSet<PostItData> m_postIts = new HashSet<PostItData>();

		public override int Count {
            get { return m_postIts.Count; }
        }

		public override IEnumerator<PostItData> GetEnumerator() {
			return m_postIts.GetEnumerator();
		}

        public override void Clear() {
            m_postIts.Clear();
            base.Clear();
        }

        public void Parse() {
            Parse(Generator.Instance);
        }

		private class Generator : GeneratorBase<PostItAsset> {
            static public readonly Generator Instance = new Generator();

			public override bool TryCreateBlock(IBlockParserUtil inUtil, PostItAsset inPackage, TagData inId, out PostItData outBlock) {
				string fullName = inUtil.TempBuilder.Append(inUtil.Position.FileName)
                    .Append('-')
                    .Append(inId.Id)
                    .Flush();

                outBlock = new PostItData(fullName);
                inPackage.m_postIts.Add(outBlock);
                return true;
			}
		}

        #if UNITY_EDITOR

		[ScriptedExtension(1, "postit")]
        private class Importer : ImporterBase<PostItAsset> {
        }

        #endif // UNITY_EDITOR
	}

}