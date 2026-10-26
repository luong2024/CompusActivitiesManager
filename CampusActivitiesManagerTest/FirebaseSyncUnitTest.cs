using CampusActivitiesManager.Data;

namespace CampusActivitiesManagerTest
{
    [TestFixture]
    public class FirebaseSyncUnitTest
    {
        [Test]
        public void FirebaseConfig_Parameters_AreConfiguredForCloudFirestore()
        {
            Assert.That(FirebaseConfig.ProjectId, Is.EqualTo("campusacmanage"));
            Assert.That(FirebaseConfig.ClientEmail, Does.Contain("campusacmanage.iam.gserviceaccount.com"));
            Assert.That(FirebaseConfig.FirestoreBaseUrl, Does.Contain("campusacmanage/databases/(default)/documents"));
            Assert.That(FirebaseConfig.PrivateKey, Does.StartWith("-----BEGIN PRIVATE KEY-----"));
            Assert.That(FirebaseConfig.TokenUri, Does.Contain("oauth2.googleapis.com"));
        }
    }
}
