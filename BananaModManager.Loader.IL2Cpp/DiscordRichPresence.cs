using System.Collections.Generic;
using System.Text;
using DiscordRPC;
using Flash2;
using Framework.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BananaModManager.Loader.IL2Cpp
{
    public static class DiscordRichPresence
    {
        // MainGame Character Dictionary - Character.eKind => Character Name
        public static Dictionary<Chara.eKind, string> Characters = new Dictionary<Chara.eKind, string>
        {
            {Chara.eKind.Aiai, "Aiai"},
            {Chara.eKind.Meemee, "Meemee"},
            {Chara.eKind.Baby, "Baby"},
            {Chara.eKind.Gongon, "Gongon"},
            {Chara.eKind.Yanyan, "Yanyan"},
            {Chara.eKind.Doctor, "Doctor"},
            {Chara.eKind.Jam, "Jam"},
            {Chara.eKind.Jet, "Jet"},
            {Chara.eKind.Sonic, "Sonic"},
            {Chara.eKind.Tails, "Tails"},
            {Chara.eKind.Kiryu, "Kiryu"},
            {Chara.eKind.Beat, "Beat"},
            {Chara.eKind.Dlc01, "Hello Kitty"},
            {Chara.eKind.Dlc02, "Morgana"},
            {Chara.eKind.Dlc03, "Suezo"},
            {Chara.eKind.GameGear, "the Game Gear"},
            {Chara.eKind.SegaSaturn, "the Sega Saturn"},
            {Chara.eKind.Dreamcast, "the Sega Dreamcast"}
        };

        // Race Course Dict - Find Course GameObject.name => Course Name
        public static Dictionary<PgRaceDefine.ePgRaceCourseKind, string> Races = new Dictionary<PgRaceDefine.ePgRaceCourseKind, string>
        {
            {PgRaceDefine.ePgRaceCourseKind.JungleCircuit, "Jungle Circuit"},
            {PgRaceDefine.ePgRaceCourseKind.CaptivatingBananaRoad, "Charming Banana Road"},
            {PgRaceDefine.ePgRaceCourseKind.AquaOfRoad, "Aqua Offroad"},
            {PgRaceDefine.ePgRaceCourseKind.LovelyAmusementPark, "Lovely Heart Ring"},
            {PgRaceDefine.ePgRaceCourseKind.FrozenHighway, "Frozen Highway"},
            {PgRaceDefine.ePgRaceCourseKind.TicktackGearSlope, "Clock Tower Hill"},
            {PgRaceDefine.ePgRaceCourseKind.SkyDownTown, "Sky Downtown"},
            {PgRaceDefine.ePgRaceCourseKind.AtchitchiCircuit, "Cannonball Circuit"},
            {PgRaceDefine.ePgRaceCourseKind.PipeWarpTunnel, "Pipe Warp Tunnel"},
            {PgRaceDefine.ePgRaceCourseKind.SinkingStreet, "Submarine Street"},
            {PgRaceDefine.ePgRaceCourseKind.SpeedDessert, "Speed Desert"},
            {PgRaceDefine.ePgRaceCourseKind.SpaceColony, "Starlight Highway"}
        };

        // Billiards Dict - Find Billiards.eRule => Rules Full Name
        public static Dictionary<PartyGameDef.Billiards.eRule, string> BilliardsRules = new Dictionary<PartyGameDef.Billiards.eRule, string>
        {
            {PartyGameDef.Billiards.eRule.NineBall, "US Nine-Ball"},
            {PartyGameDef.Billiards.eRule.JapanNineBall, "Japan Nine-Ball"},
            {PartyGameDef.Billiards.eRule.Rotation, "Rotation"},
            {PartyGameDef.Billiards.eRule.EightBall, "Eight-Ball"}
        };

        // Boat Dict - Find Course GameObject.name => Course Name
        public static Dictionary<string, string> BoatCourses = new Dictionary<string, string>
        {
            {"BoatCourse01(Clone)", "Flower Garden Path"},
            {"BoatCourse02(Clone)", "Wooden Arch River"},
            {"BoatCourse03(Clone)", "Water Dragon Route"}
        };

        // Shot Dict - Find Stage GameObject.name => Stage Name
        public new static Dictionary<string, string> ShotStages = new Dictionary<string, string>
        {
            {"ShotBg01(Clone)", "Jungle Wars"},
            {"ShotBg02(Clone)", "Ocean Attack"},
            {"ShotBg03(Clone)", "Planet Monkeys"}
        };

        // Dogfight Dict - Find Stage GameObject.name => Stage Name
        public new static Dictionary<string, string> DogfightStages = new Dictionary<string, string>
        {
            {"DogfightStage01(Clone)", "Turtle Island"},
            {"DogfightStage02(Clone)", "Midair Battlefield"},
            {"DogfightStage03(Clone)", "Space Monkey Wars"}
        };

        // Baseball Dict - GameObject.name => Stadium Name
        public static Dictionary<string, string> BaseballStadium = new Dictionary<string, string>
        {
            {"BaseballStage01(Clone)", "Banana Stadium"},
            {"BaseballStage02(Clone)", "Monkey Dome"}
        };

        // Tennis Dict - bgObj.name => Court Name
        public static Dictionary<string, string> TennisCourts = new Dictionary<string, string>
        {
            {"TennisCourt01(Clone)", "Monkey Jungle"},
            {"TennisCourt02(Clone)", "Kingdom Stadium"},
            {"TennisCourt03(Clone)", "Paradise Street"}
        };

        public static void BananaManiaRPC(DiscordRpcClient discordClient)
        {
            // TODO: This is a performance nightmare holy fuck
            // Did big improvements, but it probably still needs a more significant rewrite
            // All the FindObjectOfType calls are very heavy and tank the performance
            // Same with null comparisons to some extent
            // Either fix or split off to a separate mod ASAP
            try
            {
                var discordRichPresence = new RichPresence();
                var scene = SceneManager.GetActiveScene().name;
                switch (scene)
                {
                    case "Title":
                        discordRichPresence.Details = "At the Title Screen!";
                        break;
                    case "MainMenu":
                        discordRichPresence.Details = "Browsing the Main Menu!";
                        break;
                    default:
                        if (SceneManager.GetSceneByName("PgRace").isLoaded)
                        {
                            var raceSequence = GameObject.FindObjectOfType<PgRaceSequence>();
                            if (raceSequence == null)
                                return;

                            var raceCourse = GameObject.FindObjectOfType<PgRaceCourse>();
                            if (raceCourse == null)
                                return;

                            var raceKind = raceCourse._courseKind_k__BackingField;

                            discordRichPresence.Details = "Currently playing Monkey Race!";
                            discordRichPresence.State = $"Racing on {Races[raceKind]}.";
                        }
                        else if (SceneManager.GetSceneByName("PgFight").isLoaded)
                        {
                            var fightSequence = GameObject.FindObjectOfType<PgFightSequence>();
                            if (fightSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Fight!";
                            discordRichPresence.State = $"Fighting in Round {fightSequence.m_mainCnt + 1}";
                        }
                        else if (SceneManager.GetSceneByName("PgTarget").isLoaded)
                        {
                            var targetSequence = GameObject.FindObjectOfType<PgTargetSequence>();
                            if (targetSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Target!";
                            discordRichPresence.State = $"Flying through Round {targetSequence._currentRoundIndex_k__BackingField + 1}";
                        }
                        else if (SceneManager.GetSceneByName("PgBilliards").isLoaded)
                        {
                            var billiardsSequence = GameObject.FindObjectOfType<PgBilliardsSequence>();
                            if (billiardsSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Billiards!";
                            discordRichPresence.State = $"Playing {BilliardsRules[GameObject.FindObjectOfType<PgBilliardsRuleInfo>().m_Rule]}.";
                        }
                        else if (SceneManager.GetSceneByName("PgBowling").isLoaded)
                        {
                            var bowlingSequence = GameObject.FindObjectOfType<PgBowlingSequence>();
                            if (bowlingSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Bowling!";
                        }
                        else if (SceneManager.GetSceneByName("PgGolf").isLoaded)
                        {
                            var golfSequence = GameObject.FindObjectOfType<PgGolfSequence>();
                            if (golfSequence.m_golfMode == null)
                                return;

                            var holeCount = golfSequence.m_golfMode.m_holeNo;
                            discordRichPresence.Details = "Currently playing Monkey Golf!";
                            discordRichPresence.State = $"Currently on Hole {holeCount + 1}.";
                        }
                        else if (SceneManager.GetSceneByName("PgBoat").isLoaded)
                        {
                            var boatSequence = GameObject.FindObjectOfType<PgBoatSequence>();
                            if (boatSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Boat!";
                            discordRichPresence.State = $"On the waters of {BoatCourses[GameObject.FindObjectOfType<PgBoatCourse>().name]}.";
                        }
                        else if (SceneManager.GetSceneByName("PgShot").isLoaded)
                        {
                            var shotSequence = GameObject.FindObjectOfType<PgShotSequence>();
                            if (shotSequence == null)
                                return;

                            var stageName = shotSequence.bgObj.name;
                            discordRichPresence.Details = "Currently playing Monkey Shot!";
                            discordRichPresence.State = $"Shooting through {ShotStages[stageName]}.";
                        }
                        else if (SceneManager.GetSceneByName("PgDogfight").isLoaded)
                        {
                            var dogfightSequence = GameObject.FindObjectOfType<PgDogfightSequence>();
                            if (dogfightSequence == null)
                                return;

                            discordRichPresence.Details = "Currently playing Monkey Dogfight!";
                            discordRichPresence.State = $"In aerial combat on {DogfightStages[dogfightSequence.m_CurrentStageObj.name]}";
                        }
                        else if (SceneManager.GetSceneByName("PgFutsal").isLoaded)
                        {
                            var futsalGameInfo = GameObject.FindObjectOfType<PgFutsalGameInfo>();
                            if (futsalGameInfo == null)
                                return;

                            var lScore = (int)futsalGameInfo.score[0];
                            var rScore = (int)futsalGameInfo.score[1];

                            discordRichPresence.Details = "Currently playing Monkey Soccer!";
                            discordRichPresence.State = $"Current Game: {lScore} - {rScore}";
                        }
                        else if (SceneManager.GetSceneByName("PgBaseball").isLoaded)
                        {
                            var baseballSequence = GameObject.FindObjectOfType<PgBaseballSequence>();
                            if (baseballSequence.gameData.oneTeamData == null)
                                return;

                            var lScore = baseballSequence.gameData.oneTeamData.totalScore;
                            var rScore = baseballSequence.gameData.twoTeamData.totalScore;

                            discordRichPresence.Details = $"Playing Monkey Baseball at {BaseballStadium[GameObject.FindObjectOfType<PgBaseballStage>().name]}!";
                            discordRichPresence.State = $"Current Game Score: {lScore} - {rScore}";
                        }
                        else if (SceneManager.GetSceneByName("PgTennis").isLoaded)
                        {
                            var tennisScore = GameObject.FindObjectOfType<PgTennisScore>();
                            if (tennisScore.m_PointCount == null)
                                return;

                            var lScore = GameObject.Find("Score0").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0, 1);
                            var rScore = GameObject.Find("Score1").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0, 1);

                            discordRichPresence.Details = $"Playing Monkey Tennis on {TennisCourts[GameObject.FindObjectOfType<PgTennisCourt>().name]}!";
                            discordRichPresence.State = $" Current Game Score: {lScore} - {rScore}";
                        }
                        else if (SceneManager.GetSceneByName("MainGame").isLoaded)
                        {
                            var mainGameStage = GameObject.FindObjectOfType<MainGameStage>();
                            if (mainGameStage == null)
                                return;

                            if (GameObject.FindObjectOfType<Player>() == null)
                                return;

                            var modeName = "";
                            var stageName = "";
                            var mode = mainGameStage.m_GameKind.ToString();
                            switch (mode)
                            {
                                case "Story":
                                    modeName = $"In Story Mode: {GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text}";
                                    break;
                                case "Challenge":
                                    modeName = $"In {GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text}";
                                    break;
                                case "Practice":
                                    modeName = "In Practice Mode:";
                                    break;
                                case "TimeAttack":
                                    modeName = $"Ranking Challenge: {GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text}";
                                    break;
                                case "Reverse":
                                    modeName = "In Reverse Mode:";
                                    break;
                                case "Rotten":
                                    modeName = "In Dark Banana Mode:";
                                    break;
                                case "Golden":
                                    modeName = "In Golden Banana Mode:";
                                    break;
                            }
                            stageName = mainGameStage.m_mgStageDatum.stageName;
                            // Set Presence Details as {Mode}: {Stage Name}
                            // Set Presence State as "Playing as {Character}
                            var player = GameObject.Find("Player(Clone)");
                            // Every stage name is stored in all caps
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = $"{modeName} {CapitalizeStageName(stageName)}",
                                State = $"Playing as {Characters[(player.GetComponent<Player>().charaKind)]}"
                            });
                        }
                        break;
                }
                discordClient.SetPresence(discordRichPresence);
            }
            catch
            {
                // Errors here are not a huge deal I guess???
            }
        }

        private static string CapitalizeStageName(string stageName)
        {
            // Managed to shrink that big ass function into this
            var corrected = new StringBuilder();
            var capital = true;

            foreach (var character in stageName)
            {
                corrected.Append(capital ? character : char.ToLower(character));
                capital = character == ' ' || character == '-';
            }

            return corrected.ToString();
        }
    }
}
