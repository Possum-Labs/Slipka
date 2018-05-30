Feature: Injection
	Make sure that recorded responses work correctly

Background:
	Given the Slipka Proxy 'P1' for http://PossumLabs.com'
	
Scenario: Get Hello World
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Hello World   | 200    | GET    |
	And the GET call 'C1' to Host 'P1.ProxyHost' and Path 'test'
	Then the call 'C1' has the values
	| Response Body | Status |
	| Hello World   | 200    |

Scenario: Error response
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Some Error    | 500    | GET    |
	And the GET call 'C1' to Host 'P1.ProxyHost' and Path 'test'
	Then the call 'C1' has the values
	| Response Body | Status |
	| Some Error    | 500    |

Scenario Outline: Other methods
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method   |
	| PossumLabs.com | test | Hello World   | 200    | <Method> |
	And the <Method> call 'C1' to Host 'P1.ProxyHost' and Path 'test' and Body '<Body>'
	Then the call 'C1' has the values
	| Response Body | Status |
	| Hello World   | 200    |
Examples: 
| Method  | Body | Description                                                                |
| GET     | null | Requests data from a specified resource                                    |
| POST    | {}   | Submits data to be processed to a specified resource                       |
| HEAD    | null | Same as GET but returns only HTTP headers and no DomainObjectcument body             |
| PUT     | {}   | Uploads a representation of the specified URI                              |
| DELETE  | null | Deletes the specified resource                                             |
| OPTIONS | null | Returns the HTTP methods that the server supports                          |
| CONNECT | null | Converts the request connection to a transparent TCP/IP tunnel             |
| PATCH   | {}   | The HTTP PATCH request method applies partial modifications to a resource. |

Scenario: Delay
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method | Delay |
	| PossumLabs.com | test | {}            | 200    | GET    | 1000  |
	And the GET call 'C1' to Host 'P1.ProxyHost' and Path 'test'
	Then the call 'C1' has the values
	| Response Body | Status | Duration |
	| Hello World   | 200    | > 1000   |

Scenario: Reporting
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Hello World   | 200    | GET    |
	And the GET call 'C1' to Host 'P1.ProxyHost' and Path 'test'
	Then The Proxy 'P1' has the calues
	| Calls |
	| 1     |
	And Proxy 'P1' Call '0' has the Values
	| Overwritten | Path |
	| true        | test |

Scenario: Make sure that we DomainObjectn't forward the original call
	Given the Slipka Proxy 'P2' for 'P1.Url'
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Bad           | 200    | GET    |
	Given the Proxy 'P2' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Hello World   | 200    | GET    |
	And the GET call 'C1' to Host 'P2.ProxyHost' and Path 'test'
	Then The Proxy 'P0' has the calues
	| Calls |
	| 0     |
	Then The Proxy 'P2' has the calues
	| Calls |
	| 1     |

Scenario: intercept by header

Scenario: multi part DomainObjectwnlaod simulation
