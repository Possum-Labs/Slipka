Feature: Injection
	Make sure that recorded responses work correctly

Background:
	Given the Slipka Proxy
	| var | Host                  | Destination           |
	| P1  | http://localhost:4445 | http://PossumLabs.com |

Scenario: Get Hello World
	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | Hello World      | 200        | GET    |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1' has the values
	| Response Content | StatusCode |
	| Hello World      | 200        |

Scenario: Error response
	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method |
	| /test | my error         | 500        | GET    |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then 'C1' has the values
	| Response Content | StatusCode |
	| my error         | 500        |

Scenario Outline: Other methods
	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method   |
	| /test | <Body>           | 200        | <Method> |
	And the Call
	| var | Host        | Path | Method   |
	| C1  | P1.ProxyUri | test | <Method> |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1' has the values
	| Response Content | StatusCode |
	| <Body>           | 200        |
Examples: 
| Method  | Body | Description                                                                |
| GET     | null | Requests data from a specified resource                                    |
| POST    | {}   | Submits data to be processed to a specified resource                       |
#| HEAD    | null | Same as GET but returns only HTTP headers and no DomainObjectcument body   | #empty body
| PUT     | {}   | Uploads a representation of the specified URI                              |
| DELETE  | null | Deletes the specified resource                                             |
| OPTIONS | null | Returns the HTTP methods that the server supports                          |
| PATCH   | {}   | The HTTP PATCH request method applies partial modifications to a resource. |

Scenario: Delay
	Given the Proxy 'P1' intercepts the calls
	| Uri   | Response Content | StatusCode | Method | Duration |
	| /test | Hello World      | 200        | GET    | 1000     |
	And the Call
	| var | Host        | Path | Method |
	| C1  | P1.ProxyUri | test | GET    |
	When the Call 'C1' is executed
	Then close the Proxy 'P1'
	And 'C1' has the values
	|Duration |
	|> 1000   |

	@ignore
Scenario: Reporting
	Given the Proxy 'P1' intercepts the calls
	| Host           | Path | Response Body | Status | method |
	| PossumLabs.com | test | Hello World   | 200    | GET    |
	And the GET call 'C1' to Host 'P1.ProxyHost' and Path 'test'
	Then close the Proxy 'P1'
	And The Proxy 'P1' has the calues
	| Calls |
	| 1     |
	And Proxy 'P1' Call '0' has the Values
	| Overwritten | Path |
	| true        | test |
	
	@ignore
Scenario: Make sure that we DomainObjectn't forward the original call

	@ignore
Scenario: intercept by header

	@ignore
Scenario: multi part DomainObjectwnlaod simulation
