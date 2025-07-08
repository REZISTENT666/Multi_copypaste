using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.Requests;
using Google.Api;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;
using System;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;


namespace Multi_cp
{
    public class www {
        
        internal FirebaseAuthClient client;
        internal FirebaseAuthConfig cfg;
        internal Google.Cloud.Firestore.FirestoreDb db;
        internal FirestoreClientBuilder fireclient;

        public www() {
            //ustalamy cfg firebase authentication
            cfg = new FirebaseAuthConfig {
                ApiKey = "AIzaSyDrgqmEjAlMdJ2QuChNTqUW3ecVfGw5TL8",
                AuthDomain = "multi-platform-copypaste.firebaseapp.com",
                Providers =  new FirebaseAuthProvider[] {
                 new EmailProvider(),
                 
                },
                
                UserRepository = new FileUserRepository("Firebase_credentials"),



            }; 
            client = new FirebaseAuthClient(cfg);
            
        }
        
        // konfigurujemy tworzenie instancji firestoredb, poniewaz sdk dla c# nie jest wystarczajaco  rozbudowany, ta czesc kodu jest ze stackoverflow, tylko zmienilem duza czesc aby 
        //bylo to zgodne z moja apka 
        public FirestoreDb CreateDbWithEmailAndPassword() {
            var callcreds = CallCredentials.FromInterceptor(async (Context, Metadata) =>
            {
                var token = await client.User.GetIdTokenAsync(forceRefresh: false);
                Metadata.Clear();
                Metadata.Add("authorization", $"Bearer {token}");

            });

            var credentials = ChannelCredentials.Create(new SslCredentials(), callcreds);
            var grpcChannel = new Grpc.Core.Channel("firestore.googleapis.com", credentials);
            var grcpClient = new Firestore.FirestoreClient(grpcChannel);
            var firestoreClient = new FirestoreClientImpl(grcpClient, FirestoreSettings.GetDefault(),null);
            return FirestoreDb.Create("multi-platform-copypaste",firestoreClient);
        }
        
    }

}
        


