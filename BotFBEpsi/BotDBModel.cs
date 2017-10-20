namespace BotFBEpsi
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.Bot.Connector;

    public partial class BotDBModel : DbContext
    {
        public BotDBModel()
            : base("name=BotDBModel")
        {
        }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }

        internal void enregistrementUtilisateur(Activity activity)
        {
                //si l'id n'existe pas, alors on cré un nouvel utilisateur
                if (!Users.Any(user => user.Id == activity.From.Id))
                {
                    User nouvelUtilisateur = new User { Id = activity.From.Id, EnvironnementName = activity.ChannelId, Pseudo = activity.From.Name };
                    Users.Add(nouvelUtilisateur);
                    SaveChanges();
                }
            
        }

        public bool enregistrementMessage(Activity activity)
        {
            try
            {
                Message message = new Message { Content = activity.Text, Date = DateTime.Now, UserId = activity.From.Id, BotMessage = false };
                Messages.Add(message);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void enregistrementBot(Activity activity, string reponseBot)
        {
            Message message = new Message { Content = reponseBot, Date = DateTime.Now, UserId = activity.From.Id, BotMessage = true };
            Messages.Add(message);
            SaveChanges();
        }
    }
}
