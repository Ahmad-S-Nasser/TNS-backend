using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Infrastructure.Persistence;

public sealed class UserManagementDbContext : MongoDbContext
{
    public IMongoCollection<User>         Users         => WriteCollection<User>("users");
    public IMongoCollection<User>         UsersRead     => ReadCollection<User>("users");
    public IMongoCollection<ChildProfile> Children      => WriteCollection<ChildProfile>("children");
    public IMongoCollection<ChildProfile> ChildrenRead  => ReadCollection<ChildProfile>("children");
    public IMongoCollection<DoctorProfile> Doctors      => WriteCollection<DoctorProfile>("doctor_profiles");
    public IMongoCollection<RoleDefault>  RoleDefaults  => WriteCollection<RoleDefault>("role_defaults");

    public UserManagementDbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        RegisterMap<User>();
        RegisterMap<ChildProfile>();
        RegisterMap<DoctorProfile>();
        RegisterMap<RoleDefault>();
    }

    private void RegisterMap<T>() where T : class
    {
        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<T>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                var idMember = cm.GetMemberMap("Id");
                if (idMember != null)
                {
                    idMember.SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.String));
                }
            });
        }
    }
}
