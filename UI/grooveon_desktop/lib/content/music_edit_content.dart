import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';

class MusicEditContent extends StatelessWidget {
  const MusicEditContent();

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
            "Edit music",
            style: TextStyle(
              fontSize: 28,
              fontWeight: FontWeight.w800,
              color: MusicScreen.textColor,
            ),
          ),
          SizedBox(height: 16),
          Text("Ovdje će ići tabela ili forma za uređivanje postojećih podataka."),
        ],
      ),
    );
  }
}