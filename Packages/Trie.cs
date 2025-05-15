using System;
using System.Collections.Generic;

namespace Trie
{
	#region Node
	public class Node<T> : IDisposable
	{
		#region Variables
		private Dictionary<char, List<Node<T>>> _children = new();
		private List<T> _items = new();
		private int _idx;
		#endregion // Variables

		#region Properties
		public IReadOnlyDictionary<char, List<Node<T>>> Children => _children;
		public IReadOnlyList<T> Items => _items;
		public int Index => _idx;
		#endregion // Properties

		#region Initialization
		public Node(int index) => _idx = index;
		#endregion // Initialization

		#region Indexer
		public List<Node<T>> this[char c]
		{
			get => _children.ContainsKey(c) ? _children[c] : null;
			set => _children[c] = value;
		}
		#endregion // Indexer

		#region Public Methods
		public void AddItem(T item) => _items.Add(item);
		#endregion // Public Methods

		#region Dispose
		public virtual void Dispose()
		{
			if (_children != null)
			{
				foreach (var nodes in _children.Values)
				{
					foreach (var node in nodes)
						node.Dispose();
					nodes.Clear();
				}
			}
			_children?.Clear();
			_children = null;

			_items?.Clear();
			_items = null;
		}

		public virtual void Clear()
		{
			_children?.Clear();
			_items?.Clear();
		}
		#endregion // Dispose
	}
	#endregion // Node

	#region Trie
	public sealed class Trie<T> : Node<T>
	{
		#region Variables
		private List<T> _searchResult;
		private Trie<T> _root;
		private int _currentIdx;
		private TrieSettings _settings;
		#endregion // Variables

		#region Debug
		public int TotalCount => _currentIdx;
		#endregion // Debug

		#region Settings
		public struct TrieSettings
		{
			// 부분 검색
			public bool UsePartialSearch;

			// 초성 검색
			public bool UseConsonantSearch;

			public static TrieSettings Default = new()
			{
				UsePartialSearch = true,
				UseConsonantSearch = true,
			};
		}
		#endregion // Settings

		#region Create & Initialize
		public static Trie<T> CreateNew(TrieSettings settings, params Pair[] pairs) => new(settings, pairs);
		private Trie(TrieSettings settings, params Pair[] pairs) : base(0)
		{
			_root = this;
			_settings = settings;
			_searchResult = new List<T>();
			_currentIdx = 0;
			foreach (var pair in pairs)
				Insert(pair);
		}

		public void Insert(Pair pair) => Insert(pair.Key, pair.Value);

		private void Insert(string key, T value)
		{
			key = ToValidKey(key);
			InsertInternal(key);

			if (_settings.UseConsonantSearch)
			{
				// 모든 문자열이 초성인경우에만 초성검색 허용
				if (ConsonantUtil.TryConvertToConsonant(key, out var result))
					InsertInternal(result);
			}

			void InsertInternal(string insertKey)
			{
				Node<T> node = _root;
				foreach (var c in insertKey)
				{
					var child = new Node<T>(_currentIdx++);
					if (node[c] == null)
						node[c] = new List<Node<T>> { child };
					else
						node[c].Add(child);

					if (_settings.UsePartialSearch)
					{
						if (_root[c] == null)
							_root[c] = new List<Node<T>> { child };
						else
							_root[c].Add(child);
					}

					node = node[c][^1];
				}

				node.AddItem(value);
			}
		}
		#endregion // Create & Initialize

		#region Search
		public List<T> FindAll(string key)
		{
			_searchResult.Clear();

			key = ToValidKey(key);
			var results = Find(key);
			if (results.Count == 0 || string.IsNullOrWhiteSpace(key))
				return _searchResult;

			var result = FindAllLeaf(results);
			return result;
		}

		private List<Node<T>> Find(string key)
		{
			var root = this;
			var result = new List<Node<T>>();
			var checkVisit = new HashSet<int>();
			if (key.Length == 0) return result;

			FindInternal(root, key, 0);
			return result;

			void FindInternal(Node<T> t, string s, int idx)
			{
				if (idx >= s.Length) return;

				var lt = t[s[idx]];
				if (lt == null) return;

				foreach (var subT in lt)
				{
					if (idx == s.Length - 1)
					{
						if (checkVisit.Contains(subT.Index)) continue;
						checkVisit.Add(subT.Index);
						result.Add(subT);
					}
					else
						FindInternal(subT, s, idx + 1);
				}
			}
		}

		private List<T> FindAllLeaf(List<Node<T>> nodes)
		{
			var result = _searchResult;
			foreach (var node in nodes)
			{
				if (node.Items != null)
				{
					foreach (var nodeItem in node.Items)
					{
						if (result.Contains(nodeItem))
							continue;
						result.Add(nodeItem);
					}
				}

				foreach (var kvp in node.Children)
					FindAllLeaf(kvp.Value);
			}

			return result;
		}
		#endregion // Search

		#region Dispose / Clear
		public override void Dispose()
		{
			_root = null;
			_searchResult?.Clear();
			_searchResult = null;
			base.Dispose();
		}

		public override void Clear()
		{
			_searchResult?.Clear();
			base.Clear();
		}
		#endregion // Dispose / Clear

		#region Util
		private string ToValidKey(string key)
		{
			key = key.Replace(" ", string.Empty);
			key = key.ToLower();
			return key;
		}
		#endregion // Util

		#region Data
		public struct Pair
		{
			public string Key;
			public T Value;
		}
		#endregion // Data
	}
	#endregion // Trie
}
