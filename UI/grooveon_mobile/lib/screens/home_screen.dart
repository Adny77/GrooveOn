import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';
import 'package:grooveon_mobile/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/screens/ask_question_screen.dart';
import 'package:grooveon_mobile/screens/my_question_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:provider/provider.dart';

import 'package:grooveon_mobile/models/music_stat_item_response.dart';
import 'package:grooveon_mobile/models/song_response.dart';
import 'package:grooveon_mobile/providers/song_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/screens/artist_info_screen.dart';
import 'package:grooveon_mobile/screens/search_apperance_screen.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';

import '../models/mobile_home_response.dart';
import '../providers/report_provider.dart';

class HomeScreen extends StatefulWidget {
  final bool showBottomNav;

  const HomeScreen({super.key, this.showBottomNav = true});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color primaryDark = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFF3E5F5);
  static const Color textDark = Color(0xFF1C1C1C);
  static const Color pageBg = Color(0xFFF8F6FB);

  late Future<MobileHomeResponse> _homeFuture;
  late UserProvider _userProvider;
  final ReportProvider _reportProvider = ReportProvider();

  late Future<List<SongResponse>> _recommendedFuture;
  final SongProvider _songProvider = SongProvider();

  @override
  void initState() {
    super.initState();
    _homeFuture = _reportProvider.getMobileHome();
    _recommendedFuture = _songProvider.getRecommended(take: 4);
    _userProvider = Provider.of<UserProvider>(context, listen: false);
  }

  Future<void> _reload() async {
    setState(() {
      _homeFuture = _reportProvider.getMobileHome();
      _recommendedFuture = _songProvider.getRecommended(take: 4);
    });

    await Future.wait([_homeFuture, _recommendedFuture]);
  }

  void _openSearch() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => const SearchScreen()),
    );
  }

  Future<void> _openArtistInfo(MusicStatItemResponse artist) async {
    try {
      final hasPremium = await _userProvider.hasPremium();

      if (!hasPremium) {
        if (!mounted) return;

        await ConfirmDialogs.okConfirmation(
          context,
          title: "Premium Required",
          message:
              "A premium subscription is required to access artist profiles.\n\nActivate premium to continue.",
          okText: "OK",
          danger: true,
        );

        return;
      }

      if (!mounted) return;

      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (_) => ArtistInfoScreen(
            artistId: artist.id,
            artistName: artist.title,
            artistImageUrl: artist.imageUrl,
            description: null,
            playCount: artist.playCount,
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, extractErrorMessage(e));
    }
  }

  Future<void> _playRecommendedSong(SongResponse track) async {
    try {
      final playerProvider = context.read<PlayerProvider>();

      await playerProvider.playSongWithPurpose(
        request: {
          "userId": Session.userId!,
          "songId": track.id,
          "purpose": "RandomMusic",
        },
      );
    } catch (e) {
      if (!mounted) return;

      SnackbarHelper.showError(context, extractErrorMessage(e));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: pageBg,
      body: Stack(
        children: [
          SafeArea(
            child: FutureBuilder<MobileHomeResponse>(
              future: _homeFuture,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return _loadingState();
                }

                if (snapshot.hasError) {
                  return _errorState(snapshot.error.toString());
                }

                final data = snapshot.data ?? MobileHomeResponse();
                final songOfTheDay = data.songOfTheDay;
                final topArtists = data.topArtists;

                return RefreshIndicator(
                  onRefresh: _reload,
                  child: Column(
                    children: [
                      _header(),
                      const SizedBox(height: 10),
                      Expanded(
                        child: SingleChildScrollView(
                          physics: const AlwaysScrollableScrollPhysics(),
                          padding: const EdgeInsets.fromLTRB(16, 0, 16, 100),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              _banner(songOfTheDay),
                              const SizedBox(height: 20),
                              _sectionTitle("Top Artists"),
                              const SizedBox(height: 10),
                              _artists(topArtists),
                              const SizedBox(height: 20),
                              _sectionTitle("Recommended for you"),
                              const SizedBox(height: 10),
                              FutureBuilder<List<SongResponse>>(
                                future: _recommendedFuture,
                                builder: (context, recSnapshot) {
                                  if (recSnapshot.connectionState ==
                                      ConnectionState.waiting) {
                                    return const Padding(
                                      padding: EdgeInsets.symmetric(
                                        vertical: 24,
                                      ),
                                      child: Center(
                                        child: CircularProgressIndicator(
                                          color: primary,
                                        ),
                                      ),
                                    );
                                  }

                                  if (recSnapshot.hasError) {
                                    return _emptyBox(
                                      "Recommended songs are not available right now.",
                                    );
                                  }

                                  final recommended = recSnapshot.data ?? [];

                                  return _recommendedTracks(recommended);
                                },
                              ),
                              const SizedBox(height: 24),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),

          const MiniPlayerBar(),
        ],
      ),
      bottomNavigationBar: widget.showBottomNav ? _bottomNav() : null,
    );
  }

  Widget _loadingState() {
    return Column(
      children: [
        _header(),
        const SizedBox(height: 10),
        const Expanded(
          child: Center(child: CircularProgressIndicator(color: primary)),
        ),
      ],
    );
  }

  Widget _errorState(String error) {
    return Column(
      children: [
        _header(),
        const SizedBox(height: 10),
        Expanded(
          child: Center(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    Icons.error_outline,
                    size: 42,
                    color: Colors.redAccent,
                  ),
                  const SizedBox(height: 12),
                  const Text(
                    "Failed to load home data",
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    error,
                    style: const TextStyle(fontSize: 13, color: Colors.black54),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: _reload,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: primary,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(14),
                      ),
                    ),
                    child: const Text("Try Again"),
                  ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }

  Widget _header() {
  return Padding(
    padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
    child: Row(
      children: [
        Expanded(
          child: InkWell(
            borderRadius: BorderRadius.circular(22),
            onTap: _openSearch,
            child: Container(
              height: 44,
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(22),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.05),
                    blurRadius: 10,
                  ),
                ],
              ),
              padding: const EdgeInsets.symmetric(horizontal: 14),
              child: const Row(
                children: [
                  Icon(Icons.search, color: Colors.black54),
                  SizedBox(width: 10),
                  Text(
                    "Search...",
                    style: TextStyle(color: Colors.black54, fontSize: 14),
                  ),
                ],
              ),
            ),
          ),
        ),

        const SizedBox(width: 10),

        InkWell(
          onTap: () {
            Navigator.push(
              context,
              MaterialPageRoute(
                builder: (_) => const MyQuestionsScreen(),
              ),
            );
          },
          borderRadius: BorderRadius.circular(30),
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            decoration: BoxDecoration(
              color: lightPurple,
              borderRadius: BorderRadius.circular(30),
              boxShadow: [
                BoxShadow(
                  color: primaryDark.withOpacity(0.15),
                  blurRadius: 12,
                  offset: const Offset(0, 5),
                ),
              ],
            ),
            child: const Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.help_outline, color: primaryDark, size: 20),
                SizedBox(width: 6),
                Text(
                  "See Questions",
                  style: TextStyle(
                    color: primaryDark,
                    fontWeight: FontWeight.w800,
                    fontSize: 14,
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    ),
  );
}

  Widget _banner(MusicStatItemResponse? song) {
    return InkWell(
      borderRadius: BorderRadius.circular(18),
      onTap: song == null
          ? null
          : () async {
              try {
                final playerProvider = context.read<PlayerProvider>();

                await playerProvider.playSongWithPurpose(
                  request: {
                    "userId": Session.userId!,
                    "songId": song.id,
                    "purpose": "RandomMusic",
                  },
                );
              } catch (e) {
                if (!mounted) return;

                SnackbarHelper.showError(context, extractErrorMessage(e));
              }
            },
      child: Container(
        height: 130,
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(18),
          gradient: const LinearGradient(
            colors: [primary, primaryDark],
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
          ),
        ),
        child: Row(
          children: [
            Expanded(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 16, 8, 16),
                child: song == null
                    ? const Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(
                            "Song of the Day",
                            style: TextStyle(
                              color: Colors.white70,
                              fontSize: 13,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          SizedBox(height: 8),
                          Text(
                            "No data available",
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: Colors.white,
                              fontSize: 22,
                              fontWeight: FontWeight.w900,
                            ),
                          ),
                        ],
                      )
                    : Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Text(
                            "Song of the Day",
                            style: TextStyle(
                              color: Colors.white70,
                              fontSize: 13,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            song.title,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: const TextStyle(
                              color: Colors.white,
                              fontSize: 22,
                              fontWeight: FontWeight.w900,
                              height: 1.1,
                            ),
                          ),
                          const SizedBox(height: 2),
                          Row(
                            children: [
                              const Icon(
                                Icons.play_arrow_rounded,
                                color: Colors.white,
                                size: 18,
                              ),
                              const SizedBox(width: 4),
                              Text(
                                "${song.playCount} plays",
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
              ),
            ),
            Container(
              width: 92,
              height: 92,
              margin: const EdgeInsets.only(right: 12),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.18),
                borderRadius: BorderRadius.circular(16),
                image:
                    song?.imageUrl != null && song!.imageUrl!.trim().isNotEmpty
                    ? DecorationImage(
                        image: NetworkImage(song.imageUrl!),
                        fit: BoxFit.cover,
                      )
                    : null,
              ),
              child: song?.imageUrl == null || song!.imageUrl!.trim().isEmpty
                  ? const Icon(
                      Icons.music_note_rounded,
                      color: Colors.white,
                      size: 34,
                    )
                  : null,
            ),
          ],
        ),
      ),
    );
  }

  Widget _recommendedTracks(List<SongResponse> tracks) {
    if (tracks.isEmpty) {
      return _emptyBox("No recommended songs available right now.");
    }

    return GridView.builder(
      itemCount: tracks.length,
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        crossAxisSpacing: 12,
        mainAxisSpacing: 12,
        childAspectRatio: 0.95,
      ),
      itemBuilder: (_, index) {
        final track = tracks[index];

        return InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: () => _playRecommendedSong(track),
          child: Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.04),
                  blurRadius: 12,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Container(
                    decoration: BoxDecoration(
                      color: Colors.grey.shade300,
                      borderRadius: BorderRadius.circular(14),
                      image:
                          track.coverUrl != null &&
                              track.coverUrl!.trim().isNotEmpty
                          ? DecorationImage(
                              image: NetworkImage(track.coverUrl!),
                              fit: BoxFit.cover,
                            )
                          : null,
                    ),
                    child:
                        track.coverUrl == null || track.coverUrl!.trim().isEmpty
                        ? const Center(
                            child: Icon(
                              Icons.music_note_rounded,
                              size: 30,
                              color: Colors.white,
                            ),
                          )
                        : null,
                  ),
                ),
                const SizedBox(height: 10),
                Text(
                  track.title,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w800,
                    color: textDark,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  track.artistName ?? "Recommended song",
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    fontSize: 12,
                    color: Colors.black54,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                if (track.whyRecommended != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    track.whyRecommended!,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 10,
                      color: primary,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _sectionTitle(String text) {
    return Text(
      text,
      style: const TextStyle(
        fontSize: 16,
        fontWeight: FontWeight.w800,
        color: textDark,
      ),
    );
  }

  Widget _artists(List<MusicStatItemResponse> artists) {
    if (artists.isEmpty) {
      return _emptyBox("No artists available right now.");
    }

    return SizedBox(
      height: 96,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: artists.length,
        separatorBuilder: (_, __) => const SizedBox(width: 10),
        itemBuilder: (_, index) {
          final artist = artists[index];

          return InkWell(
            borderRadius: BorderRadius.circular(14),
            onTap: () => _openArtistInfo(artist),
            child: SizedBox(
              width: 68,
              child: Column(
                children: [
                  Container(
                    width: 58,
                    height: 58,
                    decoration: BoxDecoration(
                      color: Colors.grey.shade300,
                      borderRadius: BorderRadius.circular(14),
                      image:
                          artist.imageUrl != null &&
                              artist.imageUrl!.trim().isNotEmpty
                          ? DecorationImage(
                              image: NetworkImage(artist.imageUrl!),
                              fit: BoxFit.cover,
                            )
                          : null,
                    ),
                    child:
                        artist.imageUrl == null ||
                            artist.imageUrl!.trim().isEmpty
                        ? const Icon(Icons.person, color: Colors.white)
                        : null,
                  ),
                  const SizedBox(height: 6),
                  Text(
                    artist.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    textAlign: TextAlign.center,
                    style: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _emptyBox(String text) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Text(
        text,
        style: const TextStyle(
          color: Colors.black54,
          fontSize: 13,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }

  Widget _bottomNav() {
    return Container(
      height: 70,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        boxShadow: [BoxShadow(color: Color(0x11000000), blurRadius: 10)],
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _navItem(Icons.home, true),
          _navItem(Icons.person_outline, false),
          _navItem(Icons.favorite_border, false),
          _navItem(Icons.grid_view_rounded, false),
        ],
      ),
    );
  }

  Widget _navItem(IconData icon, bool active) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: active
          ? const BoxDecoration(color: primary, shape: BoxShape.circle)
          : null,
      child: Icon(icon, color: active ? Colors.white : Colors.black54),
    );
  }
}
