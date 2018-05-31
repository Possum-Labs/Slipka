Feature: Forwarding
	Make sure that recorded responses work correctly

Background:
	Given the Slipka Proxy
	| var | Host                  | Destination           |
	| P1  | http://localhost:4445 | http://PossumLabs.com |

Scenario: Forwarding happy path
	Given the Slipka Proxy
	| var | Host                  | Destination |
	| P2  | http://localhost:4445 | P1.ProxyUri |

	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P2.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1' has the values
	| Response Content | StatusCode |
	| Hello World      | 200        |