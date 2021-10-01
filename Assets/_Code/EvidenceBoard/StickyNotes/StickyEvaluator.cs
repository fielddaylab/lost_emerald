using System.Collections.Generic;
using BeauUtil;

namespace Shipwreck
{

	public class StickyEvaluator {
        private HashSet<StickyAsset> m_packages = new HashSet<StickyAsset>();
        private Dictionary<StringHash32, StickyRootEvaluator> m_roots = new Dictionary<StringHash32, StickyRootEvaluator>();

		public delegate bool RootSolvedPredicate(StringHash32 rootId);

        public void Load(StickyAsset package) {
            if (m_packages.Add(package)) {
                package.Parse();

                foreach(var data in package) {
                    foreach(var root in data.RootIds) {
                        GetEvaluator(root, true).Add(data);
                    }
                }
            }
        }
        
        public void Unload(StickyAsset package) {
            if (m_packages.Remove(package)) {
                foreach(var data in package) {
                    foreach(var root in data.RootIds) {
                        GetEvaluator(root, false)?.Remove(data);
                    }
                }

                package.Clear();
            }
        }

        public StickyInfo Evaluate(StringHash32 rootId, List<StringHash32> chain, RootSolvedPredicate alreadySolved) {
            return GetEvaluator(rootId, false)?.Evaluate(chain, alreadySolved);
        }

        public StickyInfo Evaluate(StringHash32 rootId, StringHash32[] chain, RootSolvedPredicate alreadySolved) {
            return GetEvaluator(rootId, false)?.Evaluate(chain, alreadySolved);
        }

        private StickyRootEvaluator GetEvaluator(StringHash32 rootId, bool create) {
            StickyRootEvaluator evaluator;
            if (!m_roots.TryGetValue(rootId, out evaluator) && create) {
                evaluator = m_roots[rootId] = new StickyRootEvaluator();
            }
            return evaluator;
        }
	}

}