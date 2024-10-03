#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Elympics.Tests
{
    [Category("ElympicsTournament")]
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ElympicsTournamentTest : ElympicsMonoBaseTest
    {
        public override string SceneName => "ElympicsTournamentTestScene";
        private ElympicsTournament? _sut;
        private Guid _owner = Guid.Parse("000000-0000-0000-0000-000000000001");
        private Guid _participant1 = Guid.Parse("000000-0000-0000-0000-000000000002");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene(SceneName);
            yield return new WaitUntil(() => ElympicsTournament.Instance != null);
            _sut = ElympicsTournament.Instance;
            Assert.NotNull(_sut);
        }

        [UnityTest]
        public IEnumerator Test() => UniTask.ToCoroutine(async () =>
        {
            Debug.Log("This is Test");
            Debug.Log("This is Test");
        });

        private TournamentInfo CreateTournament(DateTimeOffset created, DateTimeOffset start, DateTimeOffset end)
        {
            return new TournamentInfo
            {
                LeaderboardCapacity = 5,
                Name = "Test",
                OwnerId = _owner,
                State = TournamentState.Created,
                CreateDate = created,
                StartDate = start,
                EndDate = end,
                Participants = new List<Guid>()
                {
                    _participant1
                }
            };
        }
    }
}
