using System.Collections.Generic;
using BeauUtil;

namespace Shipwreck
{

	public class PostItEvaluator {
        private HashSet<PostItAsset> m_packages = new HashSet<PostItAsset>();
        private Dictionary<StringHash32, PostItRootEvaluator> m_roots = new Dictionary<StringHash32, PostItRootEvaluator>();

        public void Load(PostItAsset package) {
            if (m_packages.Add(package)) {
                package.Parse();

                foreach(var data in package) {
                    foreach(var root in data.RootIds) {
                        GetEvaluator(root, true).Add(data);
                    }
                }
            }
        }
        
        public void Unload(PostItAsset package) {
            if (m_packages.Remove(package)) {
                foreach(var data in package) {
                    foreach(var root in data.RootIds) {
                        GetEvaluator(root, true).Add(data);
                    }
                }

                package.Clear();
            }
        }

        public PostItData Evaluate(StringHash32 rootId, List<StringHash32> chain) {
            return GetEvaluator(rootId, false)?.Evaluate(chain);
        }

        public PostItData Evaluate(StringHash32 rootId, StringHash32[] chain) {
            return GetEvaluator(rootId, false)?.Evaluate(chain);
        }

        private PostItRootEvaluator GetEvaluator(StringHash32 rootId, bool create) {
            PostItRootEvaluator evaluator;
            if (!m_roots.TryGetValue(rootId, out evaluator) && create) {
                evaluator = m_roots[rootId] = new PostItRootEvaluator();
            }
            return evaluator;
        }
	}

}