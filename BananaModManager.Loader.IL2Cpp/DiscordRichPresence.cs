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
            // Tried to fix a little bit but probably needs a more significant rewrite
            // All the FindObjectOfType calls are very heavy and tank the performance
            // Same with null comparisons to some extent
            // Either fix or split off to a separate mod ASAP
            try
            {
                var scene = SceneManager.GetActiveScene().name;
                switch (scene)
                {
                    case "Title":
                        discordClient.SetPresence(new RichPresence()
                        {
                            Details = "At the Title Screen!"
                        });
                        break;
                    case "MainMenu":
                        discordClient.SetPresence(new RichPresence()
                        {
                            Details = "Browsing the Main Menu!"
                        });
                        break;
                    default:
                        if (SceneManager.GetSceneByName("PgRace").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgRaceSequence>() == null)
                                return;
                            var raceKind = GameObject.FindObjectOfType<PgRaceCourse>()._courseKind_k__BackingField;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Race!",
                                State = $"Racing on {Races[raceKind]}."
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgFight").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgFightSequence>() == null)
                                return;
                            var fightCount = GameObject.FindObjectOfType<PgFightSequence>().m_mainCnt;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Fight!",
                                State = $"Fighting in Round {fightCount + 1}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgTarget").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgTargetSequence>() == null)
                                return;
                            var roundCount = GameObject.FindObjectOfType<PgTargetSequence>()._currentRoundIndex_k__BackingField;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Target!",
                                State = $"Flying through Round {roundCount + 1}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBilliards").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBilliardsSequence>() == null)
                                return;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Billiards!",
                                State = $"Playing {BilliardsRules[GameObject.FindObjectOfType<PgBilliardsRuleInfo>().m_Rule]}."
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBowling").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBowlingSequence>() == null)
                                return;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Bowling!"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgGolf").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgGolfSequence>().m_golfMode == null)
                                return;
                            var holeCount = Object.FindObjectOfType<PgGolfSequence>().m_golfMode.m_holeNo;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Golf!",
                                State = $"Currently on Hole {holeCount + 1}."
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBoat").isLoaded)
                        {
                            // PgBoatSequence._course_k__BackingField
                            if (GameObject.FindObjectOfType<PgBoatSequence>() == null)
                                return;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Boat!",
                                State = $"On the waters of {BoatCourses[GameObject.FindObjectOfType<PgBoatCourse>().name]}."
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgShot").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgShotSequence>() == null)
                                return;
                            var stageName = Object.FindObjectOfType<PgShotSequence>().bgObj.name;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Shot!",
                                State = $"Shooting through {ShotStages[stageName]}.",
                            });
                        }
                        if (SceneManager.GetSceneByName("PgDogfight").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgDogfightSequence>() == null)
                                return;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Dogfight!",
                                State = $"In aerial combat on {DogfightStages[GameObject.FindObjectOfType<PgDogfightSequence>().m_CurrentStageObj.name]}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgFutsal").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgFutsalGameInfo>() == null)
                                return;
                            var lScore = (int)GameObject.FindObjectOfType<PgFutsalGameInfo>().score[0];
                            var rScore = (int)GameObject.FindObjectOfType<PgFutsalGameInfo>().score[1];
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Soccer!",
                                State = $"Current Game: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBaseball").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBaseballSequence>().gameData.oneTeamData == null)
                                return;
                            var lScore = GameObject.FindObjectOfType<PgBaseballSequence>().gameData.oneTeamData.totalScore;
                            var rScore = GameObject.FindObjectOfType<PgBaseballSequence>().gameData.twoTeamData.totalScore;
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = $"Playing Monkey Baseball at {BaseballStadium[GameObject.FindObjectOfType<PgBaseballStage>().name]}!",
                                State = $"Current Game Score: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgTennis").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgTennisScore>().m_PointCount == null)
                                return;
                            var lScore = GameObject.Find("Score0").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0, 1);
                            var rScore = GameObject.Find("Score1").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0, 1);
                            discordClient.SetPresence(new RichPresence()
                            {
                                Details = $"Playing Monkey Tennis on {TennisCourts[GameObject.FindObjectOfType<PgTennisCourt>().name]}!",
                                State = $" Current Game Score: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("MainGame").isLoaded)
                        {
                            var modeName = "";
                            var stageName = "";
                            if (GameObject.FindObjectOfType<MainGameStage>() == null)
                                return;
                            var MGS = GameObject.FindObjectOfType<MainGameStage>().gameObject;
                            if (GameObject.FindObjectOfType<Player>() == null)
                                return;
                            var mode = MGS.GetComponent<MainGameStage>().m_GameKind.ToString();
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
                            stageName = MGS.GetComponent<MainGameStage>().m_mgStageDatum.stageName;
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
            }
            catch
            {

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
