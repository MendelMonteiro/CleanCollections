CleanCollections
================

List, Dictionary and Stack which do not add GC pressure when performing any operation.

Operations should have the same order of complexity as in their BCL counterparts.

+ List
  - 3 growth strategies implemented Incremental, Doubling and Exponential
  - Indeces are permanent and do not represent the nth element in the list
  - Add is overridden, via the IIndexedList interface, such that it returns the index at which the item was added
  - Indeces which were removed can be re-used

+ Dictionary
  - Grows underlying bucket structure exponentially
  - Re-hashes in-place

TODO: 

1. Dictionary
  - Enumerator
  - What is the memory locality impact of using reference type Entries as opposed to value types (assuming we fill with new instances when growing)?

2. Lists
  - Should access by index offset to take into account deleted items or not? (potential impact on perf)

3 Queue