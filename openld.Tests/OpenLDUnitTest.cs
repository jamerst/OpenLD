using System.Collections.Generic;

using AutoMapper;

using openld.Mapping;
using openld.Models;

namespace openld.Tests {
    public class OpenLDUnitTest {
        protected static readonly TestFixture _fixture = new TestFixture();

        protected static readonly IMapper _mapper = new MapperConfiguration(cfg => {
            cfg.AddProfile<LabelProfile>();
            cfg.AddProfile<RiggedFixtureProfile>();
            cfg.AddProfile<StructureProfile>();
        }).CreateMapper();

        protected static User[] testUsers = {
            new User {
                Id = "user1",
                UserName = "user1",
                Email = "userEmail1"
            },
            new User {
                Id = "user2",
                UserName = "user2",
                Email = "userEmail2"
            }
        };

        protected static Fixture testFixture = new Fixture {
            Id = "testFixture1",
            Name = "testFixture1",
            Manufacturer = "testManf1",
            Type = new FixtureType { Id = "testType1", Name = "testType1" },
            Power = 100,
            Weight = 3.5F,
            Modes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture1_Mode1",
                    Name = "testFixture1_Mode1",
                    Channels = 10
                },
                new FixtureMode {
                    Id = "testFixture1_Mode2",
                    Name = "testFixture1_Mode2",
                    Channels = 20
                },
            }
        };

        protected static StructureType[] testStructureTypes = {
            new StructureType {
                Id = "testStructureType1",
                Name = "testStructureType1"
            },
            new StructureType {
                Id = "testStructureType2",
                Name = "testStructureType2"
            },
            new StructureType {
                Id = "testStructureTypeBar",
                Name = "Bar"
            }
        };

        protected static Drawing[] testDrawings = {
            new Drawing {
                Id = "testDrawing1",
                Title = "testDrawing1",
                Owner = testUsers[0],
                Views = new List<View> {
                    new View {
                        Id = "testDrawing1_View1",
                        Name = "testDrawing1_View1",
                        Structures = new List<Structure> {
                            new Structure {
                                Id = "testDrawing1_View1_Struct1",
                                Name = "testDrawing1_View1_Struct1",
                                Geometry = new Geometry {
                                    Points = new List<Point> {
                                        new Point {x = 1, y = 1},
                                        new Point {x = 2, y = 2}
                                    }
                                },
                                Fixtures = new List<RiggedFixture> {
                                    new RiggedFixture {
                                        Id = "testDrawing1_View1_Struct1_RFixture1",
                                        Name = "testDrawing1_View1_Struct1_RFixture1",
                                        Position = new Point {x = 1.5, y = 1.5},
                                        Fixture = testFixture,
                                        Mode = testFixture.Modes[0],
                                        Angle = 0,
                                        Channel = 1,
                                        Address = 20,
                                        Universe = 1,
                                        Colour = "L202",
                                        Notes = ""
                                    },
                                    new RiggedFixture {
                                        Id = "testDrawing1_View1_Struct1_RFixture2",
                                        Name = "testDrawing1_View1_Struct1_RFixture2",
                                        Position = new Point {x = 1, y = 1},
                                        Fixture = testFixture,
                                        Mode = testFixture.Modes[1],
                                        Angle = 0,
                                        Channel = 2,
                                        Address = 21,
                                        Universe = 1,
                                        Colour = "",
                                        Notes = ""
                                    }
                                },
                                Type = testStructureTypes[0]
                            },
                            new Structure {
                                Id = "testDrawing1_View1_Struct2",
                                Name = "testDrawing1_View1_Struct2",
                                Geometry = new Geometry {
                                    Points = new List<Point> {
                                        new Point {x = 1, y = 1},
                                        new Point {x = 2, y = 2}
                                    }
                                },
                                Fixtures = new List<RiggedFixture> { }
                            }
                        },
                        Labels = new List<Label> {
                            new Label {
                                Id = "testDrawing1_View1_Label1",
                                Position = new Point {x = 1.0, y = 1},
                                Text = "testDrawing1_View1_Label1"
                            },
                            new Label {
                                Id = "testDrawing1_View1_Label2",
                                Position = new Point {x = 1.0, y = 1},
                                Text = "testDrawing1_View1_Label2"
                            }
                        }
                    },
                     new View {
                        Id = "testDrawing1_View2",
                        Name = "testDrawing1_View2",
                        Structures = new List<Structure>(),
                        Labels = new List<Label>()
                     }
                }
            },
            new Drawing {
                Id = "testDrawing2",
                Title = "testDrawing2",
                Owner = testUsers[0],
                Views = new List<View> {
                    new View {
                        Id = "testDrawing2_View1",
                        Name = "testDrawing2_View1",
                        Structures = new List<Structure> {
                            new Structure {
                                Id = "testDrawing2_View1_Struct1",
                                Name = "testDrawing2_View1_Struct1",
                                Geometry = new Geometry {
                                    Points = new List<Point> {
                                        new Point {x = 1, y = 1},
                                        new Point {x = 2, y = 2}
                                    }
                                },
                                Fixtures = new List<RiggedFixture> {
                                    new RiggedFixture {
                                        Id = "testDrawing2_View1_Struct1_RFixture1",
                                        Name = "testDrawing2_View1_Struct1_RFixture1",
                                        Position = new Point {x = 1.5, y = 1.5},
                                        Fixture = testFixture,
                                        Mode = testFixture.Modes[0],
                                        Angle = 0,
                                        Channel = 1,
                                        Address = 20,
                                        Universe = 1,
                                        Colour = "L202",
                                        Notes = ""
                                    }
                                }
                            }
                        },
                        Labels = new List<Label> {
                            new Label {
                                Id = "testDrawing2_View1_Label1",
                                Position = new Point {x = 1.0, y = 1},
                                Text = "testDrawing1_View1_Label1"
                            },
                            new Label {
                                Id = "testDrawing2_View1_Label2",
                                Position = new Point {x = 1.0, y = 1},
                                Text = "testDrawing1_View1_Label2"
                            }
                        }
                    }
                },
                UserDrawings = new List<UserDrawing> {
                    new UserDrawing {
                        Id = "testUserDrawing_Drawing2_User2",
                        User = testUsers[1]
                    }
                }
            }
        };

        protected static Template testTemplate = new Template {
            Id = "testDrawing1_Template",
            Drawing = testDrawings[0]
        };
    }
}