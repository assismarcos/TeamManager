### User history
- As an HR user, I want a small application to control the Team Members, to know where they are located, and to know their local currency so that I can make the correct payment using their currency

### Feature and requirements
## Member
A Member has
- - Name
- - SalaryPerYear
- - Type: it can be either an employee or a contractor.
- - - If it's a Contractor, we want to store the Duration of the contract as an integer.
- - - If it's an Employee, we need to store their Role, for instance: Software Engineer, Project Manager and so on.
- A Member can be tagged, for instance: C#, Angular, General Frontend, Seasoned Leader and so on. (Tags will likely be used as filters later, so keep that in mind)
- A Member lives in a Country. When we receive the request to create the member we should receive the "country" attribute, from it we should fetch the currency of the country that you should get from https://restcountries.com/, see the following example: https://restcountries.com/v3.1/name/brasil, where "brasil" is the name of the country. we need to store the currency together with the country information so the HR team knows which currency to pay the member.

## User
A User has
- - UserName
- - - It can be an email
- - Password
- - - Any plain text, but it must be stored as a hash for security reasons

### Missing implementation/features
- - The user passowrd must be hashed
- - Unit tests and more integration tests
- - No docker deployment. The application must be executed via IDE or donet run command

### Issues or limitations
- - The integration tests depends on a real MS SQL database to run (LocalDB, docker or regular instaled instance)
- - The integration tests are not passing due to the authentication. The authentication is being mocked but I couldn't find the reason it doesn't work.
- - I spent too much time trying to create an environment to run the integration tests without any ORM and trying to solve the problem related to the mocked authentication.