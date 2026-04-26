import 'package:flutter/material.dart';

class PublicPlaylistsScreen extends StatefulWidget {
  const PublicPlaylistsScreen({super.key});

  @override
  State<PublicPlaylistsScreen> createState() => _PublicPlaylistsScreenState();
}

class _PublicPlaylistsScreenState extends State<PublicPlaylistsScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);
  static const Color textDark = Color(0xFF1C1C1C);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(18, 18, 18, 100),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                "Playliste drugih ljudi",
                style: TextStyle(
                  color: textDark,
                  fontSize: 26,
                  fontWeight: FontWeight.w900,
                ),
              ),
              const SizedBox(height: 20),
              _emptyCard(),
            ],
          ),
        ),
      ),
    );
  }

  Widget _emptyCard() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 14,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Column(
        children: [
          Icon(
            Icons.favorite_border_rounded,
            color: primary,
            size: 52,
          ),
          const SizedBox(height: 12),
          const Text(
            "Public playlists",
            style: TextStyle(
              color: textDark,
              fontSize: 18,
              fontWeight: FontWeight.w900,
            ),
          ),
          const SizedBox(height: 6),
          const Text(
            "Ovdje će biti playliste drugih korisnika.",
            textAlign: TextAlign.center,
            style: TextStyle(
              color: Colors.black54,
              fontSize: 13,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }
}