Feature: Recording

Background:
	Given the Slipka Proxy
	| var | Host                  | Destination           |
	| P1  | http://localhost:4445 | http://PossumLabs.com |

	@debug
Scenario: Recording happy path
	Given the Slipka Proxy
	| var | Host                  | Destination |
	| P2  | http://localhost:4445 | P1.ProxyUri |

	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	Given the Proxy 'P2' records the calls
	| Uri   |
	| /test |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P2.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	When retrieving the recorded calls from Proxy 'P2' as 'RC'
	Then 'RC[0]' has the values
	| Response Content | StatusCode |
	| Hello World      | 200        |

	@ignore
Scenario: record by header

	@ignroe
Scenario: record by wildcard url

	@ignore
Scenario: record a large file


