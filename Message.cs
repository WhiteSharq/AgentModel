namespace Travelling
{
    using System;

    internal class Message
    {
        internal Message(ushort senderId)
        {
            VisitedPoints = new ushort[] { senderId };
        }

        private Message(
            int mileAge,
            ushort[] visitedPts)
        {
            MileAge = mileAge;
            VisitedPoints = visitedPts;
        }

        internal ushort Id { get; } = 666;
        internal int MileAge { get; }
        internal string Path => string.Join(" -> ", VisitedPoints); //// . $"({VisitedPointsCount},{MileAge}km) {SenderId}pt -> ";
        internal ushort SenderId => VisitedPoints[VisitedPoints.Length - 1];
        internal ushort[] VisitedPoints { get; }
        internal int VisitedPointsCount => VisitedPoints.Length;

        internal Message Update(
            ushort senderId,
            int mileAge)
        {
            if (senderId == SenderId)
            {
                throw new ArgumentException(nameof(senderId));
            }

            ////if (visited <= VisitedPointsCount ||
            ////    visited - VisitedPointsCount != 1)
            ////{
            ////    throw new ArgumentException(nameof(visited));
            ////}

            if (mileAge < MileAge)
            {
                throw new ArgumentException(nameof(mileAge));
            }

            var newVisited = new ushort[VisitedPointsCount + 1];

            VisitedPoints.CopyTo(newVisited, 0);

            newVisited[VisitedPointsCount] = senderId;

            return new Message(
                mileAge,
                newVisited);
        }
    }
}