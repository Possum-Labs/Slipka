@Slipka
@SingleBrowser
Feature: Homepage

Scenario: Mongo Db
	Given injecting
	| Uri            | Content      |
	| /MongoSettings | { Db:"test"} |
	And Navigated to the Homepage
	Then the element 'Db' has the value 'test'

Scenario: Mongo Connection String
	Given injecting
	| Path               | Content                    |
	| /api/MongoSettings | { ConnectionString:"test"} |
	And Navigated to the Homepage
	Then the element 'Connection String' has the value 'test'

Scenario: Slipka FirstPort
	Given injecting
	| Path               | Content           |
	| /api/ProxySettings | { FirstPort:5000} |
	And Navigated to the Homepage
	Then the element 'First Port' has the value '5000'

Scenario: Slipka LastPort
	Given injecting
	| Path               | Content          |
	| /api/ProxySettings | { LastPort:5000} |
	And Navigated to the Homepage
	Then the element 'Last Port' has the value '5000'

Scenario: Slipka DefaultOpenFor
	Given injecting
	| Path               | Content                      |
	| /api/ProxySettings | { DefaultOpenFor:"00:36:00"} |
	And Navigated to the Homepage
	Then the element 'Default Open Duration' has the value '36 minutes'

Scenario: Slipka MaxOpenFor
	Given injecting
	| Path                | Content                  |
	| /appi/ProxySettings | { MaxOpenFor:"00:36:00"} |
	And Navigated to the Homepage
	Then the element 'Maximun Open Duration' has the value '36 minutes'

Scenario: Slipka DefaultRetainedFor
	Given injecting
	| Path               | Content                          |
	| /api/ProxySettings | { DefaultRetainedFor:"00:36:00"} |
	And Navigated to the Homepage
	Then the element 'Default Data Retaintion' has the value '36 minutes'

Scenario: Slipka MaxRetainedFor
	Given injecting
	| Path               | Content                      |
	| /api/ProxySettings | { MaxRetainedFor:"00:36:00"} |
	And Navigated to the Homepage
	Then the element 'Maximum Data Retaintion' has the value '36 minutes'

Scenario: State none
	Given injecting
	| Path       | Content |
	| /api/State | []      |
	And Navigated to the Homepage
	Then the element 'Count' has the value 'There are no active proxies'

Scenario: State one
	Given injecting
	| Path       | Content                                          |
	| /api/State | [{Port:5000, SessionId:"test", Name:"TestName"}] |
	And Navigated to the Homepage
	Then the element 'Count' has the value '1'
	And the table contains
	| Id   | Name     | Port |
	| test | TestName | 5000 |

Scenario: State Many
	Given injecting
	| Path       | Content                                                                                           |
	| /api/State | [{Port:5000, SessionId:"test", Name:"TestName"},{Port:5005, SessionId:"test2", Name:"TestName2"}] |
	And Navigated to the Homepage
	Then the element 'Count' has the value '1'
	And the table contains
	| Id    | Name      | Port |
	| test  | TestName  | 5000 |
	| test2 | TestName2 | 5005 |

Scenario: Status good
	Given injecting
	| Path       | Content       |
	| /api/State | {Messages:[]} |
	And Navigated to the Homepage
	Then the element 'Status Icon' is 'good'

Scenario: Status Warning
	Given injecting
	| Path       | Content                                   |
	| /api/State | {Message:[{type:"warning",text:"foobar"]} |
	And Navigated to the Homepage
	Then the element 'Status Icon' is 'warning'
	And the table contains
	| Type     | Message |
	| @warning | foobar  |

Scenario: Status Error
	Given injecting
	| Path       | Content                                 |
	| /api/State | {Message:[{type:"error",text:"foobar"]} |
	And Navigated to the Homepage
	Then the element 'Status Icon' is 'error'
	And the table contains
	| Type   | Message |
	| @error | foobar  |
