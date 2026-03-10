import 'package:flutter/material.dart';

class MusicOverContent extends StatelessWidget {
  const MusicOverContent({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: SingleChildScrollView(
                    child: Column(
                      children: const [
                        _AnalyticsSection(title: "Most played"),
                        SizedBox(height: 24),
                        _AnalyticsSection(title: "Least played"),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 18),
          const _TrendingGenresCard(),
        ],
      ),
    );
  }
}

class _AnalyticsSection extends StatelessWidget {
  final String title;

  const _AnalyticsSection({required this.title});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 31,
            fontWeight: FontWeight.w800,
            color: MusicOverContent.textColor,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            color: MusicOverContent.cardColor,
            border: Border.all(color: MusicOverContent.borderColor),
          ),
          child: Column(
            children: [
              Row(
                children: const [
                  Expanded(child: _SwitchHeader(title: "Year", active: true)),
                  Expanded(child: _SwitchHeader(title: "Month", active: false)),
                ],
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 16),
                child: Row(
                  children: const [
                    Expanded(child: _MiniChartCard(title: "Top albums")),
                    SizedBox(width: 28),
                    Expanded(child: _MiniChartCard(title: "Top songs")),
                    SizedBox(width: 28),
                    Expanded(child: _MiniChartCard(title: "Top artists")),
                  ],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _SwitchHeader extends StatelessWidget {
  final String title;
  final bool active;

  const _SwitchHeader({
    required this.title,
    required this.active,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 34,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: active ? Colors.white : const Color(0xFFF2F2F4),
        border: const Border(
          bottom: BorderSide(color: MusicOverContent.borderColor),
        ),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          if (active)
            Container(
              height: 2,
              width: double.infinity,
              color: MusicOverContent.primaryColor,
            ),
          Expanded(
            child: Center(
              child: Text(
                title,
                style: TextStyle(
                  fontSize: 13,
                  fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                  color: active ? MusicOverContent.primaryColor : MusicOverContent.textColor,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _MiniChartCard extends StatelessWidget {
  final String title;

  const _MiniChartCard({required this.title});

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 140,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicOverContent.borderColor),
      ),
      child: Column(
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 13,
              color: MusicOverContent.textColor,
            ),
          ),
          const SizedBox(height: 14),
          Expanded(
            child: Center(
              child: Wrap(
                spacing: 8,
                runSpacing: 8,
                children: List.generate(
                  4,
                  (index) => Container(
                    width: 32,
                    height: 32,
                    color: const Color(0xFFE7E7EA),
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _TrendingGenresCard extends StatelessWidget {
  const _TrendingGenresCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 180,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicOverContent.borderColor),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Trending genres",
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: MusicOverContent.textColor,
            ),
          ),
          SizedBox(height: 2),
          Text(
            "this month",
            style: TextStyle(
              fontSize: 12,
              color: MusicOverContent.subTextColor,
            ),
          ),
          SizedBox(height: 14),
          _GenreItem(title: "Rock"),
          SizedBox(height: 8),
          _GenreItem(title: "Pop"),
          SizedBox(height: 8),
          _GenreItem(title: "Indie"),
          SizedBox(height: 8),
          _GenreItem(title: "Jazz"),
        ],
      ),
    );
  }
}

class _GenreItem extends StatelessWidget {
  final String title;

  const _GenreItem({required this.title});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 8,
          height: 8,
          decoration: const BoxDecoration(
            color: MusicOverContent.primaryColor,
            shape: BoxShape.circle,
          ),
        ),
        const SizedBox(width: 8),
        Text(
          title,
          style: const TextStyle(
            fontSize: 13,
            color: MusicOverContent.textColor,
          ),
        ),
      ],
    );
  }
}