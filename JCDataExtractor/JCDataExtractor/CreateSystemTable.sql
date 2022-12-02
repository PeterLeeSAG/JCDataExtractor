--https://csharp2json.io/
--https://www.convertjson.com/json-to-sql.htm

CREATE TABLE Jockey(
   id   VARCHAR(30) NOT NULL PRIMARY KEY
  ,name VARCHAR(30)
);

CREATE TABLE JockeyRaceStats(
   jockeyID  VARCHAR(30)	   not null PRIMARY KEY
  ,[year]    smallint  NOT NULL PRIMARY KEY
  ,count1st  smallint  NOT NULL
  ,count2nd  smallint  NOT NULL
  ,count3rd  smallint  NOT NULL
  ,count4th  smallint  NOT NULL
  ,count5th  smallint  NOT NULL
  ,totalRide smallint  NOT NULL
  ,stakesWon smallint  NOT NULL
);

CREATE TABLE JockeyRidingRecord(
   jockeyID    VARCHAR(30)	   not null
  ,seasonRaceIndex        int NOT NULL PRIMARY KEY -- index for the whole season
  ,raceNo       int NOT NULL -- daily no of race
  ,raceURL      VARCHAR(30)
  ,placing      VARCHAR(30)
  ,trackCourse  VARCHAR(30)
  ,distance     int NOT NULL
  ,raceClass    VARCHAR(30)
  ,going        VARCHAR(30)
  ,horse        VARCHAR(30)
  ,draw         int NOT NULL
  ,rtg          int NOT NULL
  ,trainer      VARCHAR(30)
  ,gear         VARCHAR(30)
  ,bodyWeight   int NOT NULL
  ,actualWeight int NOT NULL
);

CREATE TABLE DrawStats(
   id 		   INT  NOT NULL PRIMARY KEY
  ,raceID      INT  NOT NULL
  ,courseInfo  VARCHAR(30)
  ,hotPercentW INT  NOT NULL
  ,hotPercentP INT  NOT NULL
  ,hotPercentF INT  NOT NULL
);

CREATE TABLE DrawDetail(
  drawStatID INT NOT NULL PRIMARY KEY
  ,draw      INT NOT NULL PRIMARY KEY
  ,runners   INT NOT NULL
  ,win       INT NOT NULL
  ,[second]  INT NOT NULL
  ,third     INT NOT NULL
  ,forth     INT NOT NULL
  ,percentW  INT NOT NULL
  ,percentQ  INT NOT NULL
  ,percentP  INT NOT NULL
  ,percentF  INT NOT NULL
);

CREATE TABLE Trainer(
   id   VARCHAR(30) NOT NULL PRIMARY KEY
  ,name VARCHAR(30)
);

CREATE TABLE TrainerRaceStat(
   trainerID  VARCHAR(30)	   NOT NULL PRIMARY KEY
  ,[year]    SMALLINT  NOT NULL PRIMARY KEY
  ,count1st  SMALLINT  NOT NULL
  ,count2nd  SMALLINT  NOT NULL
  ,count3rd  SMALLINT  NOT NULL
  ,count4th  SMALLINT  NOT NULL
  ,count5th  SMALLINT  NOT NULL
  ,totalRun  SMALLINT  NOT NULL
  ,stakesWon SMALLINT  NOT NULL
);

CREATE TABLE TrainerRunnerRecord(
   trainerID   VARCHAR(30) NOT NULL PRIMARY KEY
  ,[index]      SMALLINT   NOT NULL PRIMARY KEY
  ,raceURL      VARCHAR(30)
  ,placing      VARCHAR(30)
  ,trackCourse  VARCHAR(30)
  ,distance     SMALLINT  NOT NULL
  ,raceClass    VARCHAR(30)
  ,going        VARCHAR(30)
  ,horse        VARCHAR(30)
  ,draw         SMALLINT  NOT NULL
  ,rtg          SMALLINT  NOT NULL
  ,winOdds      SMALLINT  NOT NULL
  ,jockey       VARCHAR(30)
  ,gear         VARCHAR(30)
  ,bodyWeight   SMALLINT  NOT NULL
  ,actualWeight SMALLINT  NOT NULL
  ,horseFirst   VARCHAR(30)
  ,horseSecond  VARCHAR(30)
  ,horseThird   VARCHAR(30)
);

CREATE TABLE Horse(
   id   VARCHAR(30) NOT NULL PRIMARY KEY
  ,name VARCHAR(30)
);

CREATE TABLE HorseInfo(
   horseId 					VARCHAR(30) NOT NULL PRIMARY KEY
  ,countryOrigin            VARCHAR(30)
  ,age                      SMALLINT  NOT NULL
  ,colour                   VARCHAR(30)
  ,sex                      VARCHAR(30)
  ,importType               VARCHAR(30)
  ,seasonStakes             SMALLINT  NOT NULL
  ,totalStakes              SMALLINT  NOT NULL
  ,countFirstThreeAndStarts VARCHAR(30)
  ,countStartsInPast10Races SMALLINT  NOT NULL
  ,currentStableLocation    VARCHAR(30)
  ,arrivalDate              VARCHAR(19) NOT NULL
  ,trainer                  VARCHAR(30)
  ,[owner]                  VARCHAR(30)
  ,currentRating            SMALLINT  NOT NULL
  ,startofSeasonRating      SMALLINT  NOT NULL
  ,sire                     VARCHAR(30)
  ,dam                      VARCHAR(30)
  ,damSire                  VARCHAR(30)
);

CREATE TABLE HorseSire(
	sireName				VARCHAR(30) NOT NULL 
	,horseId				VARCHAR(30) NOT NULL 
)