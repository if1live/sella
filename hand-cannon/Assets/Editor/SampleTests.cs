using UnityEngine;
using System.Collections;
using UnTest;

[TestSuite]
class MyTestSuite
{
	// called before every test        
	[TestSetup]
	void DoSetup ()
	{
	}

	// any exception is a test failure
	[Test]
	void GetResult_WithValidResult_GivesResult ()
	{
		Assert.IsTrue (ServerConfig.PORT == 15225); 
	}

}
