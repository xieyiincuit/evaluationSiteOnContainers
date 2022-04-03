namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContextSeed
{
    public async Task SeedAsync(EvaluationContext context, ILogger<EvaluationContextSeed> logger,
        IOptions<EvaluationSettings> settings, IWebHostEnvironment env)
    {
        //是Mesh则不负责Seed
        if (settings.Value.IsMeshClient) return;

        //use policy to retry seed when connect database failure
        var policy = CreatePolicy(logger, nameof(EvaluationContextSeed));

        await policy.ExecuteAsync(async () =>
        {
            var useCustomizationData = settings.Value.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;

            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(useCustomizationData
                    ? GetEvaluationCategoriesFromFile(contentRootPath, logger)
                    : GetPreConfigurationEvaluationCategory());
                await context.SaveChangesAsync();
            }

            if (!context.Articles.Any())
            {
                await context.Articles.AddRangeAsync(GetPreConfigurationEvaluationArticle());
                await context.SaveChangesAsync();
            }

            if (!context.Comments.Any())
            {
                await context.Comments.AddRangeAsync(GetPreConfigurationEvaluationComment());
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<EvaluationArticle> GetPreConfigurationEvaluationArticle()
    {
        return new List<EvaluationArticle>
        {
            new()
            {
                ArticleId = 1,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = " “侠”味十足的战斗体验",
                Content = @"在聚窟洲，在天人城，有二人正在街道上相互对峙。其一赤裸上身，头发披散，是个精壮汉子，他双手紧握一人之高的阔刀，正凝视着对面那位手持绕龙银枪，身着东瀛服饰的少女。
                青光闪动，少女抢先将长枪刺出，直指汉子左肩，不等此招变老，她便将枪变向一扫，改点右肩。那汉子倏地压身，令枪尖从头顶撩过，不等少女变招，便上扬阔刀，猛击少女胸前回防的长枪。
                这一招力道凶猛，直接将少女打到了半空，中门大开。趁此机会，借力跃至高处的汉子调整了身形，扭身催动内力，用一记燎原烬结束了这场战斗。说它简单，是因为游戏的战斗逻辑非常直白。
                像近战武器间，就存在如猜拳般简洁的招式克制关系。轻击就如同“剪刀”，它迅捷快速，是最常规的进攻手段，但却会在“石头”，也就是重击面前碰壁。这是因为重击势大力沉，多需蓄力释放，且动作自带蓝色霸体效果。处于二者之间的“布”，则是名为“振刀”的防御手段。该动作可无效重击，并顺势弹掉对方武器，但却对轻击无效。
                与这套逻辑相得益彰的，则是适当简化的操作。
                和许多动作游戏不同，该作无需搓招，连招只需反复点击鼠标即可。其中左键为横向攻击，右键为纵击。轻按轻击，长按重击。这种设计思路，极大降低了操作门槛，让人可专心“猜拳。”
                当这两套设计结合在一起之后，便让战斗充满了武侠小说中常有的见招拆招，让胜负的天秤能围绕玩家的精准判断反复倾斜。但这种战斗模式，在简化了操作和理解的同时，却也让胜负与玩家的经验和意识高度绑定，拉开了新手与老玩家的差距。
                像在黄金段位之下的场次里，我就遇到过很多不懂“收刀”的玩家。他们习惯在战斗时打满连招，常忽视连段里重击收尾的部分。令我只需掐好时机，便能轻松振刀，掌控战局。
                但在高段位场次里，面对同样的进攻，我却需要在电光火石之间进行抉择。
                这是因为设计者围绕战斗系统做出了很多的加法。令玩家能用跳跃、下蹲、闪避来打断攻击动作，进而实现收刀，并能顺势切出轻击，破除振刀状态。这衍生出了很多骗术，使振刀不再是收益最高的选项。
                总之，围绕核心战斗系统的加减法，让《永劫无间》拥有了易上手难精通的特性，也令游戏的战斗体验，带上了武侠小说才有的独特韵味。另一方面，通过将战术竞技玩法与ACT进行融合，它还淡化了技术方面的优势，让运气也能成为胜负手，使“乱拳打死老师父”成为了可能。而这种体验上的微妙平衡，也让其成为了我心中近几年来最棒的竞技网游。",
                CreateTime = DateTime.Now.AddMonths(-1),
                Description = "在《永劫无间》中，玩家能体验到具有武侠小说韵味的动作体验。丰富的兵器系统，多种元素满足你的武侠梦。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/yongjiewujian11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gameyongjie.jpg",
                CategoryTypeId = 2,
                GameId = 1,
                GameName = "永劫无间",
                JoinCount = 272,
                SupportCount = 23,
            },
            new()
            {
                ArticleId = 2,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "宫崎英高再次跌上神坛",
                Content = @"即便在几十个小时的体验过后，《艾尔登法环》的世界仍然令我意犹未尽。它在集结了以往“魂系列”所有优点的同时，还透露着巨大的野心，用开放世界这份调料以及新增的诸多玩法，让系列又来到了一个新的高度——可以说，《艾尔登法环》是From Software为玩家们献上的一份完美答卷。

                “魂”与“开放世界”的融合，让《艾尔登法环》的可探索区域面积达到了“魂系列”之最。从翠绿的平原、猩红的沼地，到梦幻般的水晶湖，玩家将邂逅风格迥异的地貌，以及各式各样设计独特的敌人。即便在通关之后，游戏中也仍然有诸多隐藏内容等待挖掘，这其中包括玩家未曾踏足过的地下城，也有一些尚不得知如何解除封印的“神秘领域”。
                并且，《艾尔登法环》的开放世界没有任何公式化的感觉，它不会给玩家待办列表和清单“教你怎么玩”。其引导方式更像《塞尔达传说：旷野之息》，只会在赐福与NPC的对话中给予玩家少量的提示后再由我们自行选择探索路线。你甚至可以跳过前期许多主线BOSS，直接奔向下一个领域开始探索之旅。
                  
                在这个世界探索时，玩家的体验会比以往“轻松”不少。
                  
                一方面，本作赐福（即《黑暗之魂》中的篝火）的数量很多，多到吓人，且一些在野外游荡的精英怪与BOSS附近，也有着名为“玛莉卡楔石”的“存档点”，供玩家死亡后在此处直接复活——这些设计，玩家 基本告别了此前“魂系列”中死亡后疲于跑图的坐牢体验，时刻能感受到宫崎英高的怜悯。
                  
                另一方面，“灵马”的加入，也加快了玩家的探索节奏。在马背上，你可以在开阔的场景中驰骋、可以绕过怪群跑酷、可以飞跃立体的山崖，甚至可以和野外精英怪、BOSS玩一波“骑马与砍杀”——总之这位 新伙伴，能让玩家在探索时更加的便捷、灵活。
                  
                本作的开放世界舞台“交界地”庞大却并不空旷，各式各样的填充物散落其中。除了分布各地的不同敌人和可供搜集内容外，游戏中还有一些名称炫酷，表现力和战斗力完全不输主线的野外BOSS，以需花点时间去攻破，但奖励也会很丰厚的敌方据点。
                  
                可能有些朋友听到“清理据点”就要PTSD了，但请放心，本作大部分敌人聚集地，都有着配合环境的独特设计。不同“据点”的场景风格天差地别，其中既有居民会从眼中喷射火焰的腐化村庄，也有囚着不知名法师，布满诡异怪 物和四手人偶的幽暗墓穴。攻克们过程并不会重复、无聊，你很容易被充满“叙事性”的环境设计代入进去，不自觉思考这些据点背后的故事。同因为战思  路、场景风格的不  同，它们也从心 理和玩法层面给我提供了很多主线之外的乐趣。
                
                同时，世界各地还分布着许多隐秘的小型地牢，在关底通常会有精英怪和BOSS等待玩家，这里往往能获取一些较为独特的道具和装备。不过遗憾的是，一些小型地牢的结构太过简陋，进入之后只有一个小房间，击败所有敌人就能开宝箱走人，这种粗暴的“踢宝”式设计，多少会降低玩家的探索欲望。
                本作的开放世界舞台“交界地”庞大却并不空旷，各式各样的填充物散落其中。除了分布各地的不同敌人和可供搜集内容外，游戏中还有一些名称炫酷，表现力和战斗力完全不输主线的野外BOSS，以及需要花点时间去攻破，但奖励也会很丰厚的敌方据点。
                
                本作新增的“战灰系统”可以看做是《黑暗之魂3》战技和变质系统的融合。战灰不仅能改变武器的属性、附魔，还可以替换武器原有的战技，例如让一把平平无奇的长矛刮起风暴，或是取消盾牌原有的弹反能力，令玩家在持盾状态下使用右手武器的战技等。游戏中的战灰种类繁多，各种组合搭配也具备一定的深度，能够大幅扩展战斗的乐趣。
                而最关键的、令无数老不死人们都热泪盈眶的变化则是——如今，“魂宇宙”中膝盖能弯曲的，不再只有苇名忍者了。《艾尔登法环》中跳跃功能的加入，不仅能让玩家迈过半身高的平台，也扩充了战斗的维度，跳跃攻击有着更高的伤害与削韧，可以破坏敌人的防御，也能让烦人的飞行怪直接倒地。和《只狼》中类似，跳跃同样能代替翻滚躲避一些下段攻击，且与此同时玩家可以进行反击，从而提供比翻滚更多的输出空间——当然，仅限特定情况。
                《艾尔登法环》塑造出了一个史诗般的宏伟世界，在地图的广度与战斗系统的深度方面，都达到了系列之最，它不只是魂系列的集大成者，还在保留了“魂”最精髓内容的同时，利用开放世界让更多玩家沉浸其中，成为了一部无与伦比的杰作。在这个内容充实，充满挑战和探索乐趣的世界中冒险，对大多数玩家来说都是一件美妙且难忘的事。",
                CreateTime = DateTime.Now.AddDays(-3),
                Description = "《艾尔登法环》塑造出了一个史诗般的宏伟世界，在地图的广度与战斗系统的深度方面，都达到了系列之最。",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 1,
                ArticleImage = "articleinfo/evaluation/laotouhuan11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gamelaotouhuan.jpg",
                GameId = 2,
                GameName = "艾尔登法环",
                JoinCount = 2542,
                SupportCount = 56,
            },
            new()
            {
                ArticleId = 3,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = " 弄巧成拙的饥饿营销",
                Content = @"
                我不喜欢今年的《战地5》，尤其是现在这个时间节点的初版《战地5》。
　　             和两年前的《战地1》相比，它没有任何一个战场元素能在第一时间抓住我的眼球。遮天蔽日的齐柏林飞艇、钢铁巨兽般的装甲列车、矫健迅捷的战马——因为时代背景的关系，它们统统消失在了历史洪流当中。《战地5》所展现的战场，被我们再熟悉不过的二战机枪、大炮、坦克、战机主宰，没有丝毫的意外和惊喜。

                《战地5》最独特的地方在于两个全新的玩法模式。一个是名为“火风暴”的大逃杀玩法，结合《战地》系列在载具和大战场方面的深厚积累，它很有可能引领继《使命召唤：黑色行动4》“黑色战域”之后的又一次吃鸡模式创新。另一个则是提供动态的目标和剧情、且更加适合朋友间开黑的协同作战模式。要知道，自2011年的《战地3》以来，这个系列已经连续7年没有成熟的PVE玩法，这一回应该能很好地满足玩家间的互动社交需求。但尴尬的是，这两个模式均不包含在首发版本当中。大逃杀要等到明年3月，协同作战模式何时上线更是个未知数。
                初版《战地5》就是一个半成品——这么说虽然很不客气，但却是铁打的事实。DICE的游戏总监将延迟发售新模式的做法解释为“防止因为模式太多，以至于玩家玩不过来”。对于这个说辞，我一个字都不相信。真正的原因不外乎两个：第一，游戏压根就没做完，但迫于营收财报压力，不得不“先赚钱，后干活”。第二，“放长线，钓大鱼”，通过搞饥饿营销的方式维持玩家社群的关注度，从而在未来刺激更多的游戏内付费，或者引导玩家多充几个月的EA会员。

                “盛大行动”是目前你能玩到唯一的新模式。这是一个需要持续进行多个阶段的64人对战玩法，每个阶段在游戏中对应一天，在现实里大约会持续20分钟。随着阶段的推进，地图场景、任务目标和玩法规则也会进行改变；上一个阶段胜利方的优势，也会以某种形式继承到下一阶段当中。显然，这个模式是在尽可能还原真实的大规模战役体验——不但旷日持久、形势多变，而且每一次战斗的结果都会对后续战争状况产生影响。
                这个概念听起来或许挺激动人心，但是“盛大行动”的实际体验远远没有那么美好。不论阶段怎么变化，具体的玩法本质上依然翻来覆去是“争夺据点”、“炸毁目标”、“推进战线”、“削减对手兵力”。战斗胜负对后续阶段的影响也仅限于“更多的重生点数”、“较短的载具重生延迟”等数值规则的层面。各个阶段的地图、玩法和故事脚本全都是固定的——不但死板，而且经常会出现一些尴尬诡异、甚至是令人忍俊不禁的现象。比如在一场港口保卫战中，作为防守方的德军就算把试图登陆的英军打得落花流水，下一阶段的剧本依然会钦定德军进行战略撤退，然后转移到崇山峻岭之间进行困兽之斗……

                《战地5》在团队合作方面进行了一定程度的优化，队友之间彼此依赖的程度变得更高。这从以下三个细节可以看出端倪：第一，弹药携带量受到了更严格的限制，单兵续航能力大幅削弱；第二，即便不是专门的医疗兵，玩家之间也可以互相拯救倒下的队友；第三，在特定的据点区域，玩家可以修建铁丝网、坦克陷阱、沙包掩体等防御工事。事实上也正是因为这些微妙的规则变化，能补充弹药的支援兵变得更受欢迎，每一个玩家都更愿意积极帮助身边倒下的队友——而且就算是射击技巧较差的菜鸟玩家，也能够通过修建防御工事的形式给队友提供帮助，享受属于自己的那份乐趣。
                四个职业之间变得更具差异化，也更能在战场中各施所长。突击兵具备更快的生命恢复能力，应对危险的战场环境更加从容不迫。医疗兵除了能更快拯救倒地的队友之外，还配备有烟雾发射装置，从而为救援活动提供安全保障。支援兵能够提供弹药补给，修建防御工事的速度更快，还能用重机枪或霰弹枪提供火力压制。侦察兵则能够利用望远镜或信号枪帮队伍标记敌人位置，在信息层面上提供支援。
                今年的《使命召唤》没有战役，这让许多青睐单机游玩部分的玩家感到失望的同时，也在一定程度上衬托出了《战地5》的“良心”。不过从实际情况来看，《战地5》的战役其实存在非常严重的缩水状况。这部作品一共只有四个战役章节，每个章节只有1.5-2个小时的流程——和《战地1》相比本来就在体量上存在劣势。更过分的是，其中还有一个章节需要等到今年12月份的时候才会推出，这多半也是EA饥饿营销计划的一环。
                当前可以玩到的3个战役章节在关卡设计、游戏节奏和叙事水准上和《战地1》基本上在伯仲之间。但让我困惑的是，其中居然有两个关卡都聚焦于深入敌后的破坏活动——不但在体验上显得比较重复，而且也给我一种很强的“避重就轻”的感觉。毕竟，二战是个波澜壮阔的背景题材，《战地》系列的传统优势也刚好在于渲染气势恢宏的正面战场。可这一回《战地5》的战役偏偏显得过于小家子气。

                《战地5》是系列史上玩家配合体验最棒的一作，但是由于“火风暴”和“协同作战”两个关键模式尚未发布，当前主推的“盛大行动”又存在诸多明显的缺陷——当下这个时间节点的初版《战地5》远远没能达到我的期待。和最近的《使命召唤：黑色行动4》相比，它在体量、质量和诚意上更是相形见绌。
                想方设法维持一款服务型游戏的玩家社群活力本身无可厚非，但是正当的做法应该是在原本就已经高度完善的基础上精进、进化，而非将理所当然属于玩家的内容拆分开来搞饥饿营销。或许半年后的《战地5》会随着新模式、新内容的不断更新脱胎换骨，到时候我们也会为这篇评测更新一个“DLC”，从而完善对这款游戏的看法。",
                CreateTime = DateTime.Now.AddDays(-9),
                Description = "《战地5》是系列史上玩家配合体验最棒的一作，但当下这个时间节点的初版《战地5》远远没能达到我的期待。",
                Status = ArticleStatus.Normal,
                CategoryTypeId = 2,
                ArticleImage = "articleinfo/evaluation/zhandi511.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gamezhandiwu.jpg",
                GameId = 3,
                GameName = "战地5",
                JoinCount = 232,
                SupportCount = 13,
            },
            new()
            {
                ArticleId = 4,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "更有深度的战术竞技体验",
                Content = @"
                或许就连 Respawn（重生工作室）自己也没有想到《Apex英雄》会火成这个德行，仅用了一天时间玩家人数就突破250万，如今更是突破了2500万，甚至在Twitch上把堡垒之夜拉下了王座。当然，对于EA来说，这种自己截胡自己的操作，也不是第一次了。
                没有铺天盖地的宣传，只是一夜之间，游戏火了，这让我感到十足的意外。出于工作的原因，笔者玩过PUBG、堡垒之夜等太多五花八门的战术竞技游戏，自认为这类游戏不会再有太大的纵向延伸。但在尝试过《Apex英雄》之后，我改变了看法，它依然是一款值得一玩的好游戏。
                《Apex英雄》采用了重生工作室的大作《泰坦陨落2》的诸多设定，但逐渐缩小的安全区，只有一队笑到最后的获胜机制等，都足以说明它的本质依然一款标准的战术竞技游戏。
                《Apex英雄》是有职业划分的，每个职业拥有不同的技能，包含一个被动技能，一个战术技能和一个绝招，技能不同，在战场的定位也不同。拥有探测和获取视野以及高移动速度的寻血犬更适合当个侦察兵，走在队伍的最前沿；而作为团队奶妈的“生命线”则更适合在队伍中间，受前后队友的保护。
                虽然每个职业都有技能，但《Apex英雄》的主要战斗还是要靠枪子来解决，技能大多数都只能起到辅助作用，只有少数几个技能具有攻击性。
                
                当然，如果你能合理的运用这些技能，能在很大程度上弥补你与对手技术上的差距。比如英雄邦加罗尔的战术技能可以在战场上快速布满烟雾，寻血犬的绝招则可以透过烟雾看见对手的足迹，如果能配合使用就能成为“混烟之王”，效果堪比外挂。
                
                《Apex英雄》目前并没有单排模式，想玩就要组成三人小队，或许是“1+2”（两人开黑+一路人），或许是三个陌生人，亦或许是三人开黑。如果匹配到路人，很有可能遇到对方不开麦的情况（事实上这种自闭流玩家挺多的）。这时候就要称赞一下游戏的沟通系统了，尤其是用鼠标滚轮能一键标记敌人、装备、位置的设计，简直是互动神器，哪怕一言不发，也能从队友身上获得想要的信息。不过，在遇到紧急的情况时候，还是语音的沟通更准确，更有效率。

                相较于PUBG和《堡垒之夜》这两位前辈，《Apex英雄》的节奏要快上不少。这种节奏的变调是由多种因素引起的，首先是游戏的地图。目前游戏中只有“王者峡谷”这么一张地图，地图的面积非常小，这也就注定了玩家们选择变少，很多人被迫落地就要打架。打架又是推动游戏获胜节奏的主要手段之一，因此《Apex》落地成盒的几率也要高上不少。再加同一小队的玩家基本上不会分散降落，大多都选择抱团，所以同一个资源点里的人就更多。
                由于地图不大，所以游戏里没有配备任何载具，全程都要靠跑。如果是你个战术竞技游戏的老玩家就会知道，在面对快速移动的载具时，根本燃不起多少进攻的欲望，因为成功率太低；而在面对用脚跑圈的对手时，这场架是一定要打的。如果你不打，你们跑在前面就要被偷屁股，跑在后面，就会被人反身堵在圈外。《Apex英雄》全程都要用脚赶路，发生遭遇战的几率自然要大上许多，节奏又被加快了几分。

                同时，游戏还在利用一些设定不断的加快节奏，比如高资源区设定。在其他同类型游戏里虽然也有高资源区，但往往是多个，这也就造成了玩家们降落点并不集中。但《Apex英雄》不同，每局游戏开始时，都会在地图上选定一个资源点，变成高资源区，并且用大大的光圈标注，点里会出现成套的装备，这势必会吸引大量的玩家降落在这一个点，从而让战斗规模再次升级。另外游戏还有一个装满物资的飞艇，效果与高资源区类似。",
                CreateTime = DateTime.Now.AddYears(-1),
                Description = "《Apex英雄》是一款游戏的游戏，更快的战斗节奏带来的是更多的快感以及更小的挫败感。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/apex11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gameapex.jpg",
                CategoryTypeId = 2,
                GameId = 4,
                GameName = "Apex英雄",
                JoinCount = 4567,
                SupportCount = 123,
            },
            new()
            {
                ArticleId = 5,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "驾驶重立体机甲探索世界",
                Content = @"
                前些日子，《幻塔》与《灵笼》的联动新版本【白月破晓】吸引了不少玩家的关注，随版本上线的新拟态角色白老板、全新联动活动、以及活动副本都给玩家带来了不小的惊喜，毕竟能够亲手操控心仪的动画角色去战斗，这种体验可并不常见。
                如今距离白老板上线已经过去了一个月的时间，在首个联动角色获得玩家一致好评后，《幻塔》也顺势推出了与《灵笼》联动的第二期新角色“破晓之威 马克”，这个动画中的男主角，会给玩家带来怎样的新体验呢？

                新角色竟可以开“高达”
                与之前联动《灵笼》的第一弹类似，本次联动版本中最重要的部分还是新拟态角色与意志带来的全新机制，和配装阵容上新的可能性。《灵笼》动画主角马克的登场，也势必会与此前的白月魁产生一定程度的联动效应。
                在游戏中，虽然马克无法进入到动画里的最终形态“兽化马克”，但却可以通过召唤重立体机甲来增强自身的战斗能力。
                
                由于《幻塔》本身的世界观属于后启示录的末日世界，与《灵笼》动画设定风格比较接近。再加上游戏内无论源器、怪物都已出现过大量机甲外装，新武器“破晓”的机甲模式在这片荒野中也显得不那么突兀。
                马克的重力体机甲拥有双形态切换的技能，以及大范围覆盖的炮火轰炸效果。其中“剑形态”能使出攻击力较高的连续挥砍，最后一段也会有大范围的震地效果。而另一个“双刃剑”的形态下攻击段数和速度则会有明显的提升。这两种形态下的战斗实力并没有较大的差别，不过玩家可以通过反复切换形态来打断攻击后摇，或是依靠机甲更替的飞行时间来躲避怪物技能。
                顺带一提，只要玩家仍持有“破晓”，就可以永久保持这个重力体机甲的状态，直到主动切换武器才会从机甲状态退出。这个状态在移动中虽然多少显得有些笨重，比较容易吃到伤害，但好处在于能够收获50%的额外物理减伤，抗伤坦度还是十分理想的。

                别看这个重装机甲外观看起来霸气十足，在视觉效果上就给人一种极具压制力的感受，实际上这个新武器的辅助能力远比它的输出能力更加出色。
                一星的护盾增加坦度，三星解锁增伤，在六星状态下，武器的重力体护盾BUFF将能够提升1倍的物理抗性并附带霸体效果，最重要的是其他武器技能命中敌人或者释放连携技能的时候也会获得重力体护盾的BUFF。因此，该BUFF所能提供的大额护盾、物理抗性、霸体、35%最终伤害提升、回血以及额外充能等效果均会给予装备的剩下两个武器，为爆发型武器带来伤害的质变。

                要知道白老板恰恰就是目前《幻塔》中的高爆发武器，当这两件来自《灵珑》的武器组合在一起，所引发的化学反应也会给玩家带来新的惊喜。
                同时，本次随新拟态角色推出的马克意志套装也是目前游戏中收益较高的一套。仅在两件套的情况下就能够为全体队友套盾，并提供额外的减伤效果，四件全满的状态下则还能提供集体增伤BUFF。尽管具体数值来看，马克这套的伤害加成并不如西萝的单体加成高，但其作为游戏中唯一可以后台挂起的长时效增伤群体BUFF，可谓是后期组队副本中十分好用的意志套装。
                这套新意志的出现不仅让玩家可以带来更强的群体增益效果，在单人作战的时候，也可以让后台武器来为主手提供长期的增益状态。这种可以后台挂起的增益武器与意志，它的出现将会给游戏带来更多的战术选择空间与武器搭配的可能性，让玩家的战斗流程脱离充能、破盾、输出的单向循环。",
                CreateTime = DateTime.Now.AddDays(-14),
                Description = "《幻塔》给玩家带来了不小的惊喜，毕竟能够亲手操控心仪的动画角色去战斗，这种体验可并不常见。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/huanta11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gamehuanta.jpg",
                CategoryTypeId = 3,
                GameId = 5,
                GameName = "幻塔",
                JoinCount = 1354,
                SupportCount = 25,
            },
            new()
            {
                ArticleId = 6,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "迷失少女的硬核解谜之旅",
                Content = @"
                作为一款纯粹的解谜游戏，《永进》的解谜过程并没有让我十分享受。一方面是由于谜题的设计略为单一，很容易让人感到无聊；另一方面游戏本身缺乏一定的引导和提示，导致我常常要在一处谜题上耗费大量时间，游戏节奏也显得缓慢而拖沓。
                或许清新而简洁的画风能够让《永进》吸引到一部分玩家，但最终乐趣有限的玩法只能让游戏流于平庸。
                游戏中，我们要不停的穿梭于两个世界，一个是郁郁葱葱的恬美小岛，另一个是灰白方块组成的迷宫。随着我们渐渐揭开故事的面纱，便可得知这二者皆不是真实存在的，而是主角“小雅”的内心世界。当我们不断揭开迷宫的谜题，修复小岛上背“侵染”的区域后，小雅便会逐渐找回自己的记忆。

                游戏的故事，就在这一片片记忆碎片中展现给了玩家。在这个以亲情、陪伴为主旨的故事中，我们能够感受到制作人想要传达的情感。但同时，过于短暂且零散的叙事方式，又常常使我们还没能同小雅的经历产生共情，就被急忙忙的拖入了下一章节。最终，这些情感很遗憾的没能转化为驱使我游玩的动力，游戏的故事也并没有真正的打动我。
                解谜是《永进》中最为重要的部分，我们有90%以上的流程都要在充斥着灰白方块的迷宫世界中度过，所有的谜题都在这里等着我们。

                这些谜题难易不等，但模式十分相近——我们需要突破层层障碍，将砖块搬运至终点。其中最令人头疼的莫过于那些名为“圆头”的摄像头，我们一旦被其发现并照射到，就会被送回到上一检查点。因此，如何诱骗圆头并让它们的视线偏离我们的必经之路，成为了几乎所有解谜环节都要考核的难题。
                游戏也正是围绕着这一特点，设计出了各式各样的硬核谜题。在前几个关卡中，我们还可以通过简单的丢砖块吸引圆头注意这样简单的套路来过关。但随着流程推进，你会发现谜题变得越来越复杂——不停移动、升降的平台需要我们更有规划的制定路线；多个互相监视的圆头使得我们诱骗的难度越来越高；甚至在后几个关卡中，地图还会发生垂直方向的翻转，而我们需要确保小雅随时都能找到落脚之处……
                的确，我们不难看出这些谜题都是精心设计的，往往需要相当巧妙的思路才能将其破解。但同时也会发现一个明显的问题——几乎所有的谜题都是想方设法诱骗拦路圆头，而完全没有其他解谜思路。这也导致了后续的关卡缺乏新鲜感，越玩下去就会越发感到无聊。

                此外，由于谜题的难度变得越来越高，而游戏又不会对解谜方法做任何提示，卡关便成了家常便饭。没有攻略的笔者只得去尝试任何可能的解谜手段，有时甚至要用一个方法反复实验十数次。说实话，这个过程相当折磨人，甚至让我有些沮丧，我不确定如果自己不评测这款游戏的话是否还有耐心继续玩下去。
                总之，在玩了这款游戏之后，我意识到在解谜环节中能够得到适当的提示和引导，是多么幸福的一件事。比如劳拉在遗迹兜兜转转时嘴里不停咕哝着下一步该如何；又或是任天堂的箱庭探索中，我们在反复失败之后总能收到系统的提示。同时，我也意识到卡关带来的挫败感有多么折磨人，特别是兜兜转转了一个小时还一筹莫展之时。

                好在游戏有一个相当便利的即时存档机制，我们能够借此实现快捷的回溯。这个功能让我得以在寻找谜底的过程中免去大量的重复工作，也让我能够放心大胆的去尝试各种预想中的解谜方案。这一定程度上缓解了卡关带来的痛苦，但终究是“治标不治本”。
                《永进》是一款体量不大的解谜游戏，清新简约的画风相当讨喜，但其核心的解谜玩法却并没有超出预期的表现。后期的高难度谜题也缺乏一些适当的引导，容易造成卡关并令人产生挫败感。在叙事方面，过度碎片化、上下联系并不紧凑的叙事方式也没能让整个故事及其想要传达的情感得以深入人心。总而言之，这是一款质量中规中矩的小品游戏，如果你对解谜这一类别相当感兴趣的话，那么不妨尝试一下。",
                CreateTime = DateTime.Now.AddMonths(-2),
                Description = "清新而简洁的画风能够让《永进》吸引到一部分玩家，但最终乐趣有限的玩法只能让游戏流于平庸。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/yongjin11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gameyongjin.jpg",
                CategoryTypeId = 5,
                GameId = 7,
                GameName = "永进",
                JoinCount = 786,
                SupportCount = 14,
            },
            new()
            {
                ArticleId = 7,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "汽车爱好者的嘉年华",
                Content = @"
                在GT系列诞生的第25年，山内一典给玩家带来了最好的礼物——《GT7》。次世代主机性能的加持下，该作在外显层面迎来了大跨步提升，众多GT赛车经典元素的回归、依旧硬核且出色的驾驶手感，以及对于汽车文化优秀的展现，都让它能稳稳当当的位列当今市面上赛车游戏的第一梯队。

                作为次世代的第一款GT作品，《GT7》在画面上确实有不错的进步。本作420辆车全部为高精度建模。在光追效果的加持下，展示模式中车辆内饰锐利的渲染效果、精细的材质贴图，在自然的光照下显得非常和谐且纤毫毕现。在回放模式中车身和玻璃甚至可以反射周遭环境的光影，很多场景下已经让人分不清虚拟和现实。

                而比赛时驾驶室视角的画面表现，相比GTS来说有了很大提升，车辆的内饰、驾驶员手套服饰的材质都更加真实细腻；不同天气下的光影表现也更加自然。
                不过遗憾的是，虽然赛道的光影、建模和贴图同样更加精致，近景也足够优秀。但远处大楼、树木与观众，细看起来还是有些奇怪的“塑料感”；前面提到的光追效果，也只在拍照模式、回放模式，以及游戏中除了比赛以外的3D场景中有所体现，在比赛中，玩家并不能享受到光追效果带来的震撼。
                在反复的练习中不断调校自己的车辆，寻找每一个弯道最佳的入弯时机与速度，以秒甚至毫秒为单位突破自己的极限，是“赛道刷圈”游戏最大的魅力之一——而《GT7》可以说提供了历代最好的刷圈体验。
                得益于GT赛车过去25年的积累，本作的汽车物理性能保持了系列一贯的优秀。游戏中跑圈耗时和现实中基本一致，而在驾驶手感的拟真程度方面，《GT7》也毫无疑问位于同类游戏的第一梯队。
                在很多偏向轻度的竞速游戏中，从头到尾地板油，过弯一键漂移的做法，在GT赛车中是完全行不通的。所谓“想要开得快，首先要学会刹车”，一味加速只会让你变成赛道上最绚丽的陀螺。现实赛车中“外内外”找弯心过弯，利用前车尾流效应加速等技巧，是成为一名优秀赛车手的必修课，也是成为《GT7》老ASS必须要掌握的操作。

                评测时我分别使用了PS5手柄与图马斯特T300RS-GT方向盘。游玩过程中PS5手柄给我带来了很大的惊喜，车辆行驶在路面、路肩、草地上，都会有相对应的细腻震动反馈；自适应扳机也能让我感受到刹车时轮胎锁定的强烈震动，驾驶不同车辆，在不同路面上踩油门、刹车时，手柄扳机键的阻力反馈也不尽相同。
                而当我使用方向盘时，同样感受到了操控手感上的变化，开上路肩、撞击其他车辆、高速过弯时方向盘的震动差别很容易就能感受到，换档时的顿挫感也更加明显。加速时引擎轰鸣带来的细微颤动也通过方向盘精准地传递到我手中。

                《GT7》中方向盘对于转弯过载的反馈也更加优秀，不同失控程度的临界状态都有细腻的震动频率和反作用力的差别，让方向盘对于抓地的表现更加精确。玩家可以对于车辆的失控临界点做出更精准的判断，从而调整救车的幅度。
                总之，方向盘仍然是游玩GT赛车的不二选择，不过手柄相较于前作那迎来质变的操控手感，也让其成为了值得尝试的选项。
                当然，想要顺畅地体验上面这一切，自然离不开长时间对技术的打磨，但新玩家也无需因此打退堂鼓，《GT7》提供了较为全面的辅助功能，自动刹车、自动转向、刹车区域与行车线提示，都可以让萌新不至于早早被劝退。随着驾驶技术的增长，这些辅助选项都可以分别进行调整。让玩家能有一个较为舒适的成长曲线。
                《GT7》是对过去25年GT系列发展的一次总结，系列所有核心的优势都被保留且进一步打磨。山内一典为玩家们打造了一个内容充足且趣味十足的“汽车之城”，游戏中处处体现着其对汽车文化的热爱。虽然本作没有太大的惊喜与创新，却也无愧于“GT系列25年的集大成者”这一称谓。",
                CreateTime = DateTime.Now.AddMonths(-3),
                Description = "在GT系列诞生的第25年，山内一典给玩家带来了最好的礼物——《GT7》。《GT7》无愧于“GT系列25周年集大成者”的称谓。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/gt711.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gamegt7.jpg",
                CategoryTypeId = 4,
                GameId = 8,
                GameName = "GT7",
                JoinCount = 1131,
                SupportCount = 58,
            },
            new()
            {
                ArticleId = 8,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                NickName = "留六颗橙",
                Title = "最暴力的《最终幻想》",
                Content = @"
                回看《最终幻想》近些年的作品，不难发现它们总是在整一些“新活儿”——FF15的开放世界和即时战斗，让玩家体验到了有趣的探索与酷炫的战斗；FF7re那以传统回合制为内核的即时动作玩法，也在保留了原版味道的同时，让战斗变得更加流畅有趣；FF14更是在MMO领域大展鸿图，并成功超越前辈《魔兽世界》，成为了世界上同时在线人数最多的MMORPG。
                如今，SE与Team NINJA合作的《最终幻想：起源》（下简称《起源》），则给我们带来了以硬核动作为玩法的《最终幻想》。经过多达3次的网络测试之后，有很多玩家已经初步体验过了这款游戏，所以就其到底是不是“《仁王》新作”这一说法，我在此澄清一下——它是，但也不是。
                我相信绝大部分玩过《仁王》的玩家，在体验《起源》的过程中，会有非常浓烈的既视感，毕竟游戏中很多方面的设计， 都与《仁王》很相似。比如玩家正式开始游戏前选取关卡的页面，就非常的“仁王”。
                是的，游戏内并没有提供一个可以让我们自由探索的开放世界，相反，整个游戏流程非拆分为了若干个风格迥异的单独关卡，上一关可能你还在白雪皑皑的雪山艰难探索，下一关你就会在某个地下墓穴浴血杀敌。
                而《起源》中每个关卡的流程，是根据一个名为“魔方”的地标物来分阶段的。在探索过程中，玩家不小心死亡，便会从上一个魔方处重新开始，地图上的敌人也会重新刷新。如果发现了新的魔方，就可以更新记录点，并恢复身上的所有状态。除此之外，魔方还能提供职业加点、调配队伍等功能——没错，这就是魂系游戏的“篝火”，或者更“准确”来说是《仁王》中的“神社”。
                基于这样的地图设计，在探索过程中，玩家或多或少可以找到一些方便返回魔方的近路。但几乎每张地图的流程其实都是一本道，有时候甚至会碰到，明明前方就有个新的魔方，旁边却有一条通往上个魔方近路的诡异设计，时常让我怀疑这些近路的存在意义是什么...
                再加上主线关卡打完后，就会出现一个在同一张地图内，以相反的路线再打一遍的支线，这些关于地图和流程的种种设计，都和《仁王》如出一辙。另一给予玩家巨大《仁王》既视感的方面，则在于游戏内的装备系统。
                在《仁王》中，存在“开荒魂like，刷后开无双”的说法，所以虽然号称硬核动作游戏，但使用了相同装备养成系统的《起源》，也存在类似的情况。但由于《起源》有着不同的游戏难度，以及下文中会提到的职业系统和更低的死亡惩罚等，使得即便是在开荒期，玩家    也会某 种程   度  上，有着“开无双”的感觉。
                一旦游戏一周目结束，便会从一个类魂游戏，变成一个装备驱动的刷子游戏。游戏内装备从白到橙分为五个等级，为了获取更好的装备，而反复刷图， 应该是游戏通关后的核心玩法。如果你是一个曾经在《仁王》中，为了某个套装、甚至某个词条刷了好几百小时的人，那么当你看到每个装备上的词条，那精确到小数点的数值后，应该能唤起你很多肝痛的回忆。
                而令人遗憾的是，除了这些设计层面的内容，游戏最大的缺点也和《仁王》类似。流程后期随处可见大量堆怪的场景，《仁王》中那些靠堆砌数值来提升难度的不良回忆，让我在玩《起源》时再度体会了一遍。并且游戏内的怪物建模也少的可怜，后期鲜有新模型的怪物登场，大多都是之前怪物的换色，也让我在游玩时有了一些审美疲劳。",
                CreateTime = DateTime.Now.AddMonths(-6),
                Description = "回看《最终幻想》近些年的作品，《最终幻想：起源》是一款有着《仁王》壳子的，正统《最终幻想》系列作品。",
                Status = ArticleStatus.Normal,
                ArticleImage = "articleinfo/evaluation/zzhx11.jpg",
                DescriptionImage = "gameinfopic/gamerepo/gamezzhx.jpg",
                CategoryTypeId = 4,
                GameId = 9,
                GameName = "最终幻想：起源",
                JoinCount = 5473,
                SupportCount = 92,
            },
        };
    }

    private IEnumerable<EvaluationComment> GetPreConfigurationEvaluationComment()
    {
        return new List<EvaluationComment>
        {
            new()
            {
                CommentId = 1,
                UserId = "FB9755FE-D011-435B-BD49-C4277FEB4938".ToLower(),
                Content = "买断制游戏完全按照免费游戏的玩法来。作成免费游戏还会有零氪玩家割不了这些韭菜了，作成买断制游戏就不会有零氪玩家了谁的韭菜都得割。英雄技能还需要解锁，符文页要买，符文也要买，符文升级费用还要随等级递增。这么多消耗古币的内容然而古币获取速度呢一天肝个十几个小时能获取多少呢？还TM的取消了双币购买，皮肤只能氪金了 ntm这样还不如直接开发古币用RMB购买省的每天用那么多时间去肝古币",
                CreateTime = DateTime.Now.AddDays(-10),
                ArticleId = 1
            },
            new()
            {
                CommentId = 2,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                Content = "钱都花在广告上了，游戏内容就是单纯的武侠吃鸡，没有新意。人物动作有时候不是很流畅，打击感像是故意做出来的一边挨揍一边从容出招......虽然希望国产能强大起来，但纵容垃圾，就会让开发商不思进取靠卖情赚钱，更不可能进步了，更玩不到好的国产了。",
                CreateTime = DateTime.Now.AddDays(-1),
                ArticleId = 1
            },
            new()
            {
                CommentId = 3,
                UserId = "A71C1391-1105-4E9A-BCBB-F70467EF070C".ToLower(),
                Content = "这游戏还挺不错的 但是官方是打算把这款游戏打造成中日风格的吗 又有妖刀姬又有达摩的. 本来好好的一款中国风格文化的游戏非要和日本那些魑魅魍魉结合起来 哎",
                CreateTime = DateTime.Now.AddDays(-1),
                ArticleId = 1
            },
            new()
            {
                CommentId = 4,
                UserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                Content = "网亿你懂的，当初喊着删通行证货币的玩家们，现在已经看不到踪影～",
                CreateTime = DateTime.Now.AddDays(-3).AddMinutes(20),
                ArticleId = 1,
                IsReply = true,
                ReplyCommentId = 1,
                RootCommentId = 1
            },
            new()
            {
                CommentId = 5,
                UserId = "A71C1391-1105-4E9A-BCBB-F70467EF070C".ToLower(),
                Content = "这可是所谓的本国大厂哟，他们的行事作风难道还让你期待过？",
                CreateTime = DateTime.Now.AddDays(-1).AddMinutes(22),
                ArticleId = 1,
                IsReply = true,
                ReplyCommentId = 4,
                ReplyUserId = "8440A693-5DDF-4036-9972-BCA66A8002A3".ToLower(),
                RootCommentId = 1
            },
            new()
            {
                CommentId = 6,
                UserId = "A71C1391-1105-4E9A-BCBB-F70467EF070C".ToLower(),
                Content = "还有人给网易洗？皮肤不出的yellow点我们这种lsp会上当？ 网易那开箱世界第一低的几率， 符文系统更是连lol这个始祖级的都不用了，他一个大型多人在线pvp还拿来用，学王者荣耀啊？ 不好玩就是不好玩，这游戏就是花钱请各大平台主播宣传起来的！ 打死我都不玩！！！！！！",
                CreateTime = DateTime.Now.AddDays(-1).AddHours(2),
                ArticleId = 1,
                IsReply = true,
                ReplyCommentId = 1,
                RootCommentId = 1
            }
        };
    }

    private IEnumerable<EvaluationCategory> GetEvaluationCategoriesFromFile(string contentRootPath,
        ILogger<EvaluationContextSeed> logger)
    {
        var csvFileEvaluationCategories = Path.Combine(contentRootPath, "Setup", "EvaluationCategories.csv");

        if (!File.Exists(csvFileEvaluationCategories))
        {
            logger.LogWarning("file path:{path} can't find csv file, EvaluationCategories initialize may wrong", csvFileEvaluationCategories);
            return GetPreConfigurationEvaluationCategory();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "evaluationcategories" };
            csvheaders = GetHeaders(csvFileEvaluationCategories, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return GetPreConfigurationEvaluationCategory();
        }

        return File.ReadAllLines(csvFileEvaluationCategories)
            .Skip(1) // skip header row
            .SelectTry(CreateEvaluationCategory)
            .OnCaughtException(ex =>
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            })
            .Where(x => x != null);
    }

    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        var csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
            throw new Exception(
                $"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");

        if (optionalHeaders != null)
            if (csvheaders.Count() > requiredHeaders.Count() + optionalHeaders.Count())
                throw new Exception(
                    $"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' " +
                    $"and optional '{optionalHeaders.Count()}' headers count");

        foreach (var requiredHeader in requiredHeaders)
            if (!csvheaders.Contains(requiredHeader))
                throw new Exception($"does not contain required header '{requiredHeader}'");

        return csvheaders;
    }

    private EvaluationCategory CreateEvaluationCategory(string category)
    {
        category = category.Trim('"').Trim();

        if (string.IsNullOrEmpty(category)) throw new Exception("evaluation category Name is empty");

        return new EvaluationCategory
        {
            CategoryType = category
        };
    }

    private IEnumerable<EvaluationCategory> GetPreConfigurationEvaluationCategory()
    {
        return new List<EvaluationCategory>
        {
            new() {CategoryType = "单机"},
            new() {CategoryType = "Xbox"},
            new() {CategoryType = "独立"},
            new() {CategoryType = "网游"},
            new() {CategoryType = "手游"}
        };
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<EvaluationContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<MySqlException>().WaitAndRetryAsync(
            retries,
            retry => TimeSpan.FromSeconds(5),
            (exception, timeSpan, retry, ctx) =>
            {
                //记录重试日志
                logger.LogWarning(exception,
                    "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                    prefix, exception.GetType().Name, exception.Message, retry, retries);
            }
        );
    }
}