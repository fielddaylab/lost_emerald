﻿using System;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Blocks;
using BeauUtil.Tags;

namespace Shipwreck {

    public class StickyRootEvaluator {

        private const int MinCorrectDepth = 1;
        private const int MinHintDepth = 1;
        private const int MinIncorrectDepth = 1; //hack

        private readonly List<StickyInfo> m_correctResponses = new List<StickyInfo>();
        private readonly List<StickyInfo> m_hintResponses = new List<StickyInfo>();
        private readonly List<StickyInfo> m_incorrectResponses = new List<StickyInfo>();

        private bool m_dirty;

        public void Add(StickyInfo data) {
            switch(data.Response) {
                case StickyInfo.ResponseType.Correct:
                    m_correctResponses.Add(data);
                    break;

                case StickyInfo.ResponseType.Hint:
                    m_hintResponses.Add(data);
                    break;

                case StickyInfo.ResponseType.Incorrect:
                    m_incorrectResponses.Add(data);
                    break;
            }

            m_dirty = true;
        }

        public void Remove(StickyInfo data) {
            switch(data.Response) {
                case StickyInfo.ResponseType.Correct:
                    m_correctResponses.FastRemove(data);
                    break;

                case StickyInfo.ResponseType.Hint:
                    m_hintResponses.FastRemove(data);
                    break;

                case StickyInfo.ResponseType.Incorrect:
                    m_incorrectResponses.FastRemove(data);
                    break;
            }
        }

        public StickyInfo Evaluate(ListSlice<StringHash32> chain) {
            if (chain.Length == 0)
                return null;

            if (m_dirty) {
                m_correctResponses.Sort(SortBySpecificity);
                m_hintResponses.Sort(SortBySpecificity);
                m_incorrectResponses.Sort(SortBySpecificity);
                m_dirty = false;
            }

            foreach(var correct in m_correctResponses) {
                if (EvaluateCorrect(correct, chain)) {
                    return correct;
                }
            }

            foreach(var hint in m_hintResponses) {
                if (EvaluateNormal(hint, MinHintDepth, chain)) {
                    return hint;
                }
            }

            foreach(var incorrect in m_incorrectResponses) {
                if (EvaluateNormal(incorrect, MinIncorrectDepth, chain)) {
                    return incorrect;
                }
            }

            return null;
        }

        static private bool EvaluateCorrect(StickyInfo data, ListSlice<StringHash32> chain) {
            // correct - no support for location or prerequisites
            return EvaluateChain(chain, data.NodeIds);
        }

        static private bool EvaluateNormal(StickyInfo data, int inMinDepth, ListSlice<StringHash32> chain) {
            if (!EvaluateDepth(chain.Length, inMinDepth, data.Location)) {
                return false;
            }

            var nodeIds = data.NodeIds;
            if (nodeIds.Length == 0) {
                return true;
            }

            switch(data.Location) {

                // first - has no support for prerequisites
                case StickyInfo.LocationType.First: {
                    return nodeIds.Contains(chain[0]);
                }

                // last - check last node in chain, validate prerequisites
                case StickyInfo.LocationType.Last: {
                    var prerequisites = data.Prerequisites;
                    if (chain.Length <= prerequisites.Length) {
                        return false;
                    }

                    if (!EvaluateSingle(chain[chain.Length - 1], nodeIds)) {
                        return false;
                    }

                    if (prerequisites.Length > 0) {
                        var prereqFromChain = chain.Slice(chain.Length - 1 - prerequisites.Length, prerequisites.Length);
                        if (!EvaluateChain(prereqFromChain, prerequisites)) {
                            return false;
                        }
                    }

                    return true;
                }

                // anywhere - for each possible node, find in chain, validate prerequisites
                case StickyInfo.LocationType.Anywhere: {
                    var prerequisites = data.Prerequisites;
                    if (chain.Length <= prerequisites.Length) {
                        return false;
                    }

                    foreach(var id in nodeIds) {
                        int idIndex = chain.IndexOf(id);
                        if (idIndex < prerequisites.Length) {
                            continue;
                        }

                        if (prerequisites.Length > 0) {
                            var prereqFromChain = chain.Slice(idIndex - 1 - prerequisites.Length, prerequisites.Length);
                            if (!EvaluateChain(prereqFromChain, prerequisites)) {
                                continue;
                            }
                        }

                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        static private bool EvaluateDepth(int depth, int minDepth, StickyInfo.LocationType location) {
            switch(location) {
                case StickyInfo.LocationType.First:
                    return true;

                default:
                    return depth >= minDepth;
            }
        }

        static private bool EvaluateChain(ListSlice<StringHash32> chain, ListSlice<StringHash32> match) {
            if (chain.Length != match.Length) {
                return false;
            }

            int mask = 0;
            int fullMask = (1 << match.Length) - 1;

            foreach(var id in chain) {
                int matchIdx = match.IndexOf(id);
                if (matchIdx < 0)
                    return false;

                mask |= 1 << matchIdx;
            }

            return mask == fullMask;
        }
    
        static private bool EvaluateSingle(StringHash32 id, ListSlice<StringHash32> allowed) {
            return allowed.Contains(id);
        }
    
        static private int SortBySpecificity(StickyInfo x, StickyInfo y) {
            return y.Specificity.CompareTo(x.Specificity);
        }
    }

}