# Slipka
the untrustworthy proxy


With the shift towards services, it is imperative to test what happens when services are miss behaving. Slipka is a proxy build with testing in mind. Slipka allows you to spin up a lightweight proxy with an API call, allowing each test running in parallel to have its own proxy. But it does more:

## Injection: 
specify custom responses for specified conditions; for instance, test new response codes, or mock an API before it is implemented.

## Recording: 
save the traffic you care about; for instance, you can use this to get to a file downloaded by selenium.

## Tagging: 
provide easy reporting after you are done; for instance, you can specify different conditions for a slow GET and POST 

## Decorating: 
add identifiers to all traffic passing through the proxy; for instance, add a test identifier to find it easily in your server logs.
