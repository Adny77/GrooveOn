import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';

class MusicAddContent extends StatelessWidget {
  const MusicAddContent();

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicScreen.borderColor),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Add music",
            style: TextStyle(
              fontSize: 28,
              fontWeight: FontWeight.w800,
              color: MusicScreen.textColor,
            ),
          ),
          SizedBox(height: 16),
          Text("Ovdje će ići forma za dodavanje albuma, pjesama ili artista."),
        ],
      ),
    );
  }
}