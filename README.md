# 검색 (Trie)
- [검색 (Trie)](#검색-trie)
  - [개요](#개요)
  - [기능](#기능)
  - [API](#api)
    - [Trie](#trie)
    - [Trie.TrieSettings](#trietriesettings)
  - [사용 예시](#사용-예시)
  - [제약 사항](#제약-사항)

## 개요
Trie기반 문자열 검색 도구  
입력된 내용은 Trie 구조로 배치되고, 탐색에 사용됩니다.
## 기능
- 일반 검색  
이름 기반의 검색을 합니다.
- 초성 검색 (한글)  
한글 초성 입력에 대한 검색을 합니다.
- 부분 검색 (특정 문자열의 일부분에 대한 검색)  
검색 대상의 일부분에 대한 검색을 지원합니다.
- 검색 결과로 객체 저장  
각 검색 결과에 임의의 객체를 지정하고 탐색된 결과로부터 사용할 수 있습니다.

### Trie
API | 설명
--- | ---
static Trie\<T> **CreateNew**(TrieSettings settings, params Pair[] pairs) | 새로운 Trie 생성
void **Insert**(Pair pair) | 검색 대상 추가
List\<T> **FindAll**(string key) | 검색
void **Clear**() | Trie 제거
### Trie.TrieSettings
Trie 설정 구조체

옵션 | 설명
--- | ---
**UsePartialSearch** | 부분 검색 활성화
**UseConsonantSearch** | 초성 검색 활성화

## 사용 예시
``` csharp
// 초기화
var trie = Trie<string>.CreateNew(Trie<string>.TrieSettings.Default);

// 검색 대상 추가
var items = new string[] {"홍길동", "김철수"};
foreach (var item in items)
 trie.Insert(new Trie<string>.Pair { Key = item, value = item} );

// 검색
var result = trie.FindAll("ㅎㄱㄷ");    // result = List<string> {"홍길동"};
```

## 제약 사항
- 초성 + 문자열 검색 불가  
ex) ㅎ길동 (x)