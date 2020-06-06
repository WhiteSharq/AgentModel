using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travelling
{
    internal class Agent
    {
        private readonly object _printLock = new object();

        private ushort _firstRecievedId;

        private List<Message> _returnedMessages = new List<Message>();

        private ICollection<Agent> _subscribers;

        private BlockingCollection<Message> inbox =
            new BlockingCollection<Message>();

        internal Agent(
            ushort id,
            ICollection<Agent> subscribers = null)
        {
            Id = id;

            _subscribers = subscribers ?? new Agent[0];

            InboxAdded += OnCollectionChanged;
        }

        internal event EventHandler InboxAdded;

        internal ushort Id { get; }

        private ushort[] Pts => _subscribers?.Select(a => a.Id).ToArray();

        internal Message[] GetReturned()
        {
            return _returnedMessages
                .OrderBy(m => m.MileAge)
                .ToArray();
        }

        internal void Recieve(Message message)
        {
            Console.WriteLine(
                $"Agent {Id} recieved message from " +
                $"{message.SenderId} after {message.Path}");

            if (_firstRecievedId == 0)
            {
                _firstRecievedId = message.Id;
            }

            inbox.Add(message);

            InboxAdded?.Invoke(this, EventArgs.Empty);
        }

        internal Agent With(ICollection<Agent> subscribers)
        {
            if (subscribers.Any(s => s.Id == Id))
            {
                throw new ArgumentException(nameof(subscribers));
            }

            _subscribers = subscribers;

            return this;
        }

        private void OnCollectionChanged(
            object sender,
            EventArgs e)
        {
            var message = inbox.Take();

            var updated = Process(message);

            bool tripCommited = updated.Id == _firstRecievedId &&
                Pts.All(pt => updated.VisitedPoints.Contains(pt)) &&
                updated.VisitedPoints.Count(m => m == updated.VisitedPoints[0]) == 2;

            if (tripCommited)
            {
                lock (_printLock)
                {
                    _returnedMessages.Add(updated);

                    Console.WriteLine(_returnedMessages.Count);
                }

                return;
            }

            Send(Task.FromResult(updated));
        }

        private Message Process(Message message)
        {
            if (message.SenderId == Id)
            {
                return message;
            }

            var lastRun = Distances.From(message.SenderId, Id);

            var newMileAge = message.MileAge + lastRun;

            var updatedMessage = message.Update(Id, newMileAge);

            return updatedMessage;
        }

        private void Send(Task<Message> t)
        {
            if (t.Status != TaskStatus.RanToCompletion)
            {
                return;
            }

            var m = t.Result;

            var unvisited = _subscribers
                .Where(a => !m.VisitedPoints.Contains(a.Id))
                .ToList();

            if (!unvisited.Any())
            {
                var first = _subscribers
                    .SingleOrDefault(a =>
                        a.Id == m.VisitedPoints[0]);

                first?.Recieve(m);
            }

            foreach (var agent in unvisited)
            {
                Task.Run(() =>
                    agent.Recieve(m)
                    );
            }
        }
    }
}