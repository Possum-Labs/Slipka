Feature: Tagging Calls

Background:
	Given the Slipka Proxy
	| var | Host                  | Destination           |
	| P1  | http://localhost:4445 | http://PossumLabs.com |

Scenario: Tagging happy path
	Given the Proxy 'P1' injects the calls
	| Uri   | Response Content | StatusCode | Method |
	| test  | Hello World      | 200        | GET    |
	| other | Hello World      | 200        | GET    |
	Given the Proxy 'P1' tags the calls
	| Uri  | Tags     |
	| test | ['Test'] |
	And the Call
	| var | Host        | Path  | Method |
	| C1  | P1.ProxyUri | test  | GET    |
	| C2  | P1.ProxyUri | other | GET    |
	When the Call 'C1' is executed
	And the Call 'C2' is executed
	Then close the Proxy 'P1'
	And wait 1000 ms
	And retrieving the tagged calls from Proxy 'P1' with tag 'Test' as 'RC'
	And 'RC' has the values
	| Count |
	| 1     |
	And 'RC[0]' has the values
	| Path  | StatusCode | Tags     |
	| /test | 200        | ['Test'] |
	And retrieving the Session from Proxy 'P1' as 'S1'
	And 'S1' has the values
	| Tags     |
	| ['Test'] |