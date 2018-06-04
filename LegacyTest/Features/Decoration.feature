Feature: Decoration

Background:
	Given the Slipka Proxy
	| var | Host                  | Destination           |
	| P1  | http://localhost:4445 | http://PossumLabs.com |

Scenario: Decorations happy path
	Given the Proxy 'P1' injects the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	Given the Proxy 'P1' decorates with
	| Key        | Values            |
	| my-header  | ['Test']          |
	| my-headers | ['Test1','Test2'] |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1.Response.Headers' contains the values
	| Key        | Values            |
	| my-header  | ['Test']          |
	| my-headers | ['Test1','Test2'] |

Scenario: Decorations multiple
	Given the Proxy 'P1' injects the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	Given the Proxy 'P1' decorates with
	| Key        | Values    |
	| my-header  | ['Test']  |
	| my-header1 | ['Test1'] |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1.Response.Headers' contains the values
	| Key        | Values    |
	| my-header  | ['Test']  |
	| my-header1 | ['Test1'] |

Scenario: Decorations composit
	Given the Proxy 'P1' injects the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	Given the Proxy 'P1' decorates with
	| Key        | Values            |
	| my-headers | ['Test1','Test2'] |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1.Response.Headers' contains the values
	| Key        | Values            |
	| my-headers | ['Test1','Test2'] |
