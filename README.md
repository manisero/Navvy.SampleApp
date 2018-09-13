# Navvy.SampleApp
Sample application demonstrating usage of [Navvy](https://github.com/manisero/Navvy).
Feel free to clone the repo, run the app and play with it!

## Sample output

```
Task started.
GenerateOrders:
  Item 1 (materialized in 13,4793ms):
    WriteOrders of 1...
  Item 2 (materialized in 10,4871ms):
    WriteOrders of 1 took 516,9781ms.
  Item 1 ended after 546,3188ms.
GenerateOrders: 33%
    WriteOrders of 2...
  Item 3 (materialized in 11,0338ms):
    WriteOrders of 2 took 463,2508ms.
    WriteOrders of 3...
  Item 2 ended after 991,7187ms.
GenerateOrders: 66%
    WriteOrders of 3 took 460,9616ms.
  Item 3 ended after 924,3522ms.
GenerateOrders: 100%
      Materialization: 35,0002ms
      WriteOrders: 1441,1905ms
GenerateOrders took 1511,6062ms.
GenerateOrdersCleanup:
GenerateOrdersCleanup took 4,9996ms.
ProcessOrders:
GenerateOrdersCleanup: 100%
  Item 1 (materialized in 429,2283ms):
    CalculateProfits of 1...
    CalculateProfits of 1 took 15,6742ms.
    WriteProfits of 1...
  Item 2 (materialized in 440,0813ms):
    CalculateProfits of 2...
    CalculateProfits of 2 took 16,5711ms.
    WriteProfits of 1 took 685,4194ms.
    UpdateStats of 1...
    WriteProfits of 2...
    UpdateStats of 1 took 8,4774ms.
  Item 1 ended after 1139,7207ms.
ProcessOrders: 33%
  Item 3 (materialized in 420,5337ms):
    CalculateProfits of 3...
    CalculateProfits of 3 took 14,7212ms.
    WriteProfits of 2 took 579,4982ms.
    UpdateStats of 2...
    WriteProfits of 3...
    UpdateStats of 2 took 6,2892ms.
  Item 2 ended after 1289,3057ms.
ProcessOrders: 66%
    WriteProfits of 3 took 547,1529ms.
    UpdateStats of 3...
    UpdateStats of 3 took 6,1293ms.
  Item 3 ended after 1397,4238ms.
ProcessOrders: 100%
      Materialization: 1289,8433ms
      CalculateProfits: 46,9665ms
      UpdateStats: 20,8959ms
      WriteProfits: 1812,0705ms
ProcessOrders took 2278,9185ms.
ProcessOrdersCleanup:
ProcessOrdersCleanup took 6,3488ms.
WriteSummary:
ProcessOrdersCleanup: 100%
WriteSummary took 9,9238ms.
WriteSummary: 100%
Task ended after 3815,1533ms.
```
