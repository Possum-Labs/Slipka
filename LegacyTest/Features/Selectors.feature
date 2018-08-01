@Slipka
@SingleBrowser
Feature: Selectors


Scenario Outline: entering text inputs
	Given injecting
	| Uri   | Html   |
	| /test | <html> |
	And navigated to 'test'
	When entering '<value>' into element '<target>'
	Then the element '<target>' has the value '<value>'
Examples: 
	| description              | target | value | html                                                                                                    |
	| input for                | target | Bob   | <label for="linky">target</label><input id="linky" type="text"></input>                                 |
	| input nested             | target | Bob   | <label>target<input type="text"></input></label>                                                        |
	| input aria-label         | target | Bob   | <input type="text" aria-label="target"></input>                                                         |
	| input aria-labelledby    | t1 t2  | Bob   | <input type="text" aria-labelledby= "l1 l2"></input><label id="l1">t1</label> <label id="l2">t2</label> |
	| textarea for             | target | Bob   | <label for="linky">target</label><textarea id="linky"></textarea>                                       |
	| textarea nested          | target | Bob   | <label>target<textarea></textarea></label>                                                              |
	| textarea aria-label      | target | Bob   | <textarea aria-label="target"></textarea>                                                               |
	| textarea aria-labelledby | t1 t2  | Bob   | <textarea aria-labelledby= "l1 l2"></textarea><label id="l1">t1</label><label id="l2">t2</label>        |

Scenario Outline: entering number inputs
	Given injecting
	| Uri   | Html   |
	| /test | <html> |
	And navigated to 'test'
	When entering '<value>' into element '<target>'
	Then the element '<target>' has the value '<value>'
Examples: 
	| description     | target | value | html                                                                                                      |
	| for             | target | 42    | <label for="linky">target</label><input id="linky" type="number"></input>                                 |
	| nested          | target | 42    | <label>target<input type="number"></input></label >                                                       |
	| aria-label      | target | 42    | <input type="number" aria-label="target"></input>                                                         |
	| aria-labelledby | t1 t2  | 42    | <input type="number" aria-labelledby= "l1 l2"></input><label id="l1">t1</label> <label id="l2">t2</label> |

Scenario Outline: entering dropdown inputs
	Given injecting
	| Uri   | Html   |
	| /test | <html> |
	And navigated to 'test'
	When entering '<value>' into element '<target>'
	Then the element '<target>' has the value '<value>'
Examples: 
	| description        | target | value | html                                                                                                                                                            |
	| n for              | target | Bob   | <label for="linky">target</label><select id="linky"><option value="bad">Bad</option><option value="test">Bob</option></select>                                  |
	| n nested           | target | Bob   | <label>target<select><option value="bad">Bad</option><option value="test">Bob</option></select></label >                                                        |
	| n aria-label       | target | Bob   | <select aria-label="target"><option value="bad">Bad</option><option value="test">Bob</option></select>                                                          |
	| n name             | target | Bob   | <select name="target"><option value="bad">Bad</option><option value="test">Bob</option></select>                                                                |
	| n aria-labelledby  | t1 t2  | Bob   | <select aria-labelledby= "l1 l2"><option value="bad">Bad</option><option value="test">Bob</option></select><label id="l1">t1</label><label id="l2">t2</label>   |
	| v for              | target | Bob   | <label for="linky">target</label><select id="linky"><option value="bad">Bad</option><option value="Bob">test</option></select>                                  |
	| v nested           | target | Bob   | <label>target<select><option value="bad">Bad</option><option value="Bob">test</option></select></label >                                                        |
	| v aria-label       | target | Bob   | <select aria-label="target"><option value="bad">Bad</option><option value="Bob">test</option></select>                                                          |
	| v name             | target | Bob   | <select name="target"><option value="bad">Bad</option><option value="Bob">test</option></select>                                                                |
	| v aria-labelledby  | t1 t2  | Bob   | <select aria-labelledby= "l1 l2"><option value="bad">Bad</option><option value="Bob">test</option></select><label id="l1">t1</label><label id="l2">t2</label>   |
	| dl for             | target | Bob   | <label for="linky">target</label><input id="linky" list="o"><datalist id="o"><option value="bad"><option value="Bob"></datalist></select>                       |
	| dl nested          | target | Bob   | <label>target<input list="o"><datalist id="o"><option value="bad"><option value="Bob"></datalist></label >                                                      |
	| dl aria-label      | target | Bob   | <input aria-label="target" list="o"><datalist id="o"><option value="bad"><option value="Bob"></datalist>                                                        |
	| dl aria-labelledby | t1 t2  | Bob   | <input aria-labelledby= "l1 l2" list="o"><datalist id="o"><option value="bad"><option value="Bob"></datalist><label id="l1">t1</label><label id="l2">t2</label> |

Scenario Outline: entering radio
	Given injecting
	| Uri   | Html   |
	| /test | <html> |
	And navigated to 'test'
	When entering '<value>' into element '<target>'
	Then the element '<target>' has the value '<value>'
Examples: 
	| description  | target | value | html                                                                                       |
	| value        | target | Bob   | <input type="radio" id="i1" name="target" value="Bob"></input><label for="i1">noop</label> |
	| label for    | target | Bob   | <input type="radio" id="i1" name="target" value="noop"></input><label for="i1">Bob</label> |
	| label nested | target | Bob   | <label>Bob<input type="radio" id="i1" name="target" value="noop"></input></label>          |

Scenario Outline: clicking 
	Given injecting
	| Path  | Html   |
	| /test | <html> |
	And navigated to 'test'
	When clicking the element '<target>'
Examples: 
	| description                      | target | html                                                                                                      |
	| 0   a text                       | target | <a href = "https://www.w3schools.com/html/" >target</a>                                                   |
	| 1   a title                      | target | <a href = "https://www.w3schools.com/html/" title="target">Visit our HTML Tutorial</a>                    |
	| 2   radio label                  | target | <input type="radio" id="i1" name="rb" value="42"></input><label for="i1">target</label>                   |
	| 3   radio label                  | target | <label>target<input type="radio" name="rb" value="42"></input></label>                                    |
	| 4   radio target                 | target | <input type="radio" id="i1" name="rb" value="target"></input>                                             |
	| 5   submit input                 | submit | <input type="submit"></input>                                                                             |
	| 6   submit button                | submit | <button type="submit"></button>                                                                           |
	| 7   reset                        | reset  | <input type="reset"></input>                                                                              |
	| 8   button                       | target | <button>target</button>                                                                                   |
	| 9   button nested                | target | <label>target<button></button></label>                                                                    |
	| 10  button for                   | target | <label for="b1">target</label><button id="b1"></button>                                                   |
	| 11  button aria-label            | target | <button aria-label="target"></button>                                                                     |
	| 12  button aria-labelledby       | t1 t2  | <button aria-labelledby= "l1 l2"></button><label id="l1">t1</label> <label id="l2">t2</label>             |
	| 13  input button                 | target | <input type="button" value="target"/>                                                                     |
	| 14  input button nested          | target | <label>target<input type="button"></input></label>                                                        |
	| 15  input button for             | target | <label for="b1">target</label><input type="button" id="b1"></input>                                       |
	| 16  input button aria-label      | target | <input type="button" aria-label="target"></input>                                                         |
	| 17  input button aria-labelledby | t1 t2  | <input type="button" aria-labelledby= "l1 l2"></input><label id="l1">t1</label> <label id="l2">t2</label> |
	| 18  div button                   | target | <div role='button'>target</div>                                                                           |
	| 19  div button label             | target | <div role='button'><label>target</label></div>                                                            |oing
	| 20  div link                     | target | <div role='link'>target</div>                                                                             |
	| 21  div link label               | target | <div role='link'><label>target</label></div>                                                              |
	| 22  div menuitem                 | target | <div role='menuitem'>target</div>                                                                         |
	| 23  div menuitem label           | target | <div role='menuitem'><label>target</label></div>                                                          |

Scenario: checkboxes