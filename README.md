CleanCollections
================

TODO: 

1. Dictionary
  - using list to store buckets?  
  - How to re-use buckets?
  - What is the memory locality impact of using reference type Entries as opposed to value types (assuming we fill with new instances when growing)?

2. Lists
  - Should access by index offset to take into account deleted items or not? (potential impact on perf)